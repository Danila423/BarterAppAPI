using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarterAppAPI.Data;
using BarterAppAPI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BarterAppAPI.Controllers
{
    // Контроллер для работы с избранными объявлениями
    [Route("api/favorites")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Конструктор с DI контекста БД
        public FavoritesController(AppDbContext context)
        {
            _context = context;
        }

        // DTO для добавления в избранное
        public class FavoriteDto
        {
            public int UserId { get; set; }
            public int ListingId { get; set; }
        }

        // POST /api/favorites - добавление объявления в избранное
        [HttpPost]
        public async Task<IActionResult> AddToFavorites([FromBody] FavoriteDto dto)
        {
            // Проверка наличия пользователя
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                return NotFound(new { message = "Пользователь не найден." });
            }

            // Проверка наличия объявления
            var listingExists = await _context.Listings.AnyAsync(l => l.Id == dto.ListingId);
            if (!listingExists)
            {
                return NotFound(new { message = "Объявление не найдено." });
            }

            // Проверка, не добавлено ли уже в избранное
            var alreadyFavorite = await _context.Favorites.AnyAsync(f =>
                f.UserId == dto.UserId && f.ListingId == dto.ListingId);
            if (alreadyFavorite)
            {
                return BadRequest(new { message = "Уже в избранном." });
            }

            // Создаем запись избранного
            var favorite = new Favorite
            {
                UserId = dto.UserId,
                ListingId = dto.ListingId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            // Успешный ответ
            return Ok(new { message = "Добавлено в избранное." });
        }

        // GET /api/favorites/user/{userId} - получение избранных объявлений пользователя
        // GET /api/favorites/user/{userId} - получение избранных объявлений пользователя
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserFavorites(int userId)
        {
            // Находим записи избранного и включаем связанные объявления,
            // фильтруем так, чтобы возвращались только неархивированные объявления
            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId && !f.Listing.IsArchived)
                .Include(f => f.Listing)
                .ToListAsync();

            // Возвращаем список объявлений
            var listings = favorites.Select(f => f.Listing).ToList();
            return Ok(listings);
        }


        // DELETE /api/favorites/user/{userId}/listing/{listingId} - удаление из избранного
        [HttpDelete("user/{userId}/listing/{listingId}")]
        public async Task<IActionResult> RemoveFromFavorites(int userId, int listingId)
        {
            // Находим запись избранного
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ListingId == listingId);

            if (favorite == null)
            {
                return NotFound(new { message = "Запись в избранном не найдена." });
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            // Возвращаем сообщение об успешном удалении
            return Ok(new { message = "Удалено из избранного." });
        }
    }
}
