using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarterAppAPI.Data;
using BarterAppAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using BCrypt.Net; // Для работы с BCrypt

namespace BarterAppAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Получаем контекст БД через DI
        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/users - Получаем список всех пользователей (для отладки)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET /api/users/{id} - Получаем пользователя по id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден." });
            }
            return Ok(user);
        }

        // POST /api/users/register - Регистрация нового пользователя
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            // Если пользователь с таким email уже существует, возвращаем ошибку
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest(new { message = "Пользователь с таким email уже существует." });
            }

            // Проверяем пароль на соответствие требованиям
            if (!IsValidPassword(user.Password))
            {
                return BadRequest(new { message = "Пароль должен содержать минимум 6 символов, хотя бы одну заглавную букву, одну строчную букву и одну цифру." });
            }

            // Хешируем пароль перед сохранением
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Возвращаем созданного пользователя без пароля
            return CreatedAtAction(nameof(GetUserProfile), new { id = user.Id }, user);
        }

        // GET /api/users/profile/{id} - Получаем профиль пользователя без пароля
        [HttpGet("profile/{id}")]
        public async Task<ActionResult<User>> GetProfile(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден." });
            }
            return Ok(new
            {
                user.Id,
                user.Email,
                user.Name,
                user.Phone
            });
        }

        // PUT /api/users/update/{id} - Обновление профиля пользователя
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] User updatedUser)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден." });
            }

            // Проверяем, что новый email не занят другим пользователем
            if (await _context.Users.AnyAsync(u => u.Email == updatedUser.Email && u.Id != id))
            {
                return BadRequest(new { message = "Email уже используется другим пользователем." });
            }

            user.Name = updatedUser.Name;
            user.Phone = updatedUser.Phone;
            user.Email = updatedUser.Email;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Профиль успешно обновлен." });
        }

        // POST /api/users/validate-password - Проверка текущего пароля
        [HttpPost("validate-password")]
        public async Task<IActionResult> ValidatePassword([FromBody] PasswordValidationRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден." });
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isValid)
            {
                return Unauthorized(new { message = "Неверный пароль." });
            }

            return Ok(new { message = "Пароль подтвержден." });
        }

        // PUT /api/users/change-password/{id} - Смена пароля
        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден." });
            }

            bool isValidCurrent = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password);
            if (!isValidCurrent)
            {
                return BadRequest(new { message = "Текущий пароль неверен." });
            }

            // Проверяем новый пароль
            if (!IsValidPassword(request.NewPassword))
            {
                return BadRequest(new { message = "Новый пароль должен содержать минимум 6 символов, хотя бы одну заглавную букву, одну строчную букву и одну цифру." });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Пароль успешно изменён." });
        }

        // Метод для проверки пароля на соответствие требованиям
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return false;

            bool hasUpper = false, hasLower = false, hasDigit = false;
            foreach (var c in password)
            {
                if (char.IsUpper(c))
                    hasUpper = true;
                else if (char.IsLower(c))
                    hasLower = true;
                else if (char.IsDigit(c))
                    hasDigit = true;
            }
            return hasUpper && hasLower && hasDigit;
        }
    }
}
