using Microsoft.EntityFrameworkCore;

namespace BarterAppAPI.Models
{
    // Класс пользователя
    [Index(nameof(Email), IsUnique = true)] // Индекс для уникальности Email
    public class User
    {
        public int Id { get; set; } // Идентификатор пользователя
        public string Email { get; set; } // Email пользователя
        public string Password { get; set; } // Пароль пользователя
        public string Phone { get; set; } // Телефон пользователя
        public string Name { get; set; } // Имя пользователя
    }
}
