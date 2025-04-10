using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarterAppAPI.Data;
using BarterAppAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace BarterAppAPI.Controllers
{
    [Route("api/listings")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Получаем контекст через DI
        public ListingsController(AppDbContext context)
        {
            _context = context;
        }

        // 1) Получение всех объявлений
        // GET /api/listings/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Listing>>> GetAllListings()
        {
            var listings = await _context.Listings.ToListAsync();
            return Ok(listings); // Возвращает пустой массив, если нет данных
        }

        // 2) Загрузка изображения на сервер
        // POST /api/listings/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран");

            // Путь для сохранения файла
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Копирование файла
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Формируем URL
            var imageUrl = $"http://localhost:5000/images/{uniqueFileName}";
            // Сохраните imageUrl как PhotoPath в БД

            return Ok(new { imageUrl });
        }

        // 3) Создание нового объявления
        // POST /api/listings
        [HttpPost]
        public async Task<IActionResult> CreateListing([FromBody] Listing listing)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetListingById), new { id = listing.Id }, listing);
        }

        // 4) Получение объявления по ID
        // GET /api/listings/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetListingById(int id)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound(new { message = "Объявление не найдено." });
            }
            return Ok(listing);
        }

        // 5) Обновление объявления по ID
        // PUT /api/listings/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateListing(int id, [FromBody] Listing updated)
        {
            var existing = await _context.Listings.FindAsync(id);
            if (existing == null)
            {
                return NotFound(new { message = "Объявление не найдено." });
            }

            existing.Title = updated.Title;
            existing.Condition = updated.Condition;
            existing.Description = updated.Description;
            existing.Category = updated.Category;
            existing.Type = updated.Type;

            // Обновляем фото, если изменилось
            if (updated.PhotoPath != "-")
            {
                existing.PhotoPath = updated.PhotoPath;
            }

            // Обновляем город
            existing.City = updated.City;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Объявление успешно обновлено." });
        }

        // 6) Получение объявлений конкретного пользователя (не архивированные)
        // GET /api/listings/user/{id}
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<Listing>>> GetUserListings(int id)
        {
            var listings = await _context.Listings
                .Where(l => l.UserId == id && !l.IsArchived)
                .ToListAsync();

            return Ok(listings);
        }

        // 7) Поиск, фильтрация и выбор типа объявления
        // GET /api/listings?query=...&categories=...&type=...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Listing>>> GetListings(
            [FromQuery] string? query,
            [FromQuery] string? categories,
            [FromQuery] string? type)
        {
            var listingsQuery = _context.Listings.AsQueryable();

            // Фильтр по типу
            if (!string.IsNullOrEmpty(type))
            {
                if (type == "Нужно")
                {
                    listingsQuery = listingsQuery.Where(l => l.Type == "Нужно" || string.IsNullOrEmpty(l.Type));
                }
                else if (type == "Отдам")
                {
                    listingsQuery = listingsQuery.Where(l => l.Type == "Отдам");
                }
            }


            // Фильтр по категориям
            if (!string.IsNullOrEmpty(categories))
            {
                var catList = categories.Split(',');
                listingsQuery = listingsQuery.Where(l => catList.Contains(l.Category));
            }

            // Поиск по строке
            if (!string.IsNullOrWhiteSpace(query))
            {
                listingsQuery = listingsQuery.Where(l =>
                    l.Title.Contains(query) ||
                    l.Description.Contains(query));
            }

            var results = await listingsQuery.ToListAsync();
            return Ok(results);
        }

        // 8) Архивация объявления по ID
        // PUT /api/listings/archive/{id}
        [HttpPut("archive/{id}")]
        public async Task<IActionResult> ArchiveListing(int id)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound(new { message = "Объявление не найдено." });
            }

            listing.IsArchived = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Объявление успешно архивировано." });
        }

        // 9) Получение архивированных объявлений пользователя
        // GET /api/listings/archived/{userId}
        [HttpGet("archived/{userId}")]
        public async Task<ActionResult<IEnumerable<Listing>>> GetArchivedListings(int userId)
        {
            var archivedListings = await _context.Listings
                .Where(l => l.UserId == userId && l.IsArchived)
                .ToListAsync();

            return Ok(archivedListings);
        }
    }
}
