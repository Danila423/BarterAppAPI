using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarterAppAPI.Data;
using BarterAppAPI.Models;
using System.Threading.Tasks;
using BCrypt.Net;

namespace BarterAppAPI.Controllers
{
    // Контроллер для аутентификации
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Контекст БД
        private readonly AppDbContext _context;

        // Конструктор с зависимостью от контекста
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            // Ищем пользователя по email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

            // Если пользователь не найден, возвращаем 401
            if (user == null)
            {
                return Unauthorized(new { message = "Пользователь не найден." });
            }

            // Проверяем пароль
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);
            if (!isValidPassword)
            {
                return Unauthorized(new { message = "Неверный пароль." });
            }

            // Если всё хорошо, возвращаем данные пользователя
            return Ok(new
            {
                message = "Вход выполнен успешно.",
                user = new
                {
                    user.Id,
                    user.Email,
                    user.Name,
                    user.Phone
                }
            });
        }
    }
}
