namespace BarterAppAPI.Models
{
    // Класс запроса для входа пользователя
    public class UserLoginRequest
    {
        public string Email { get; set; } // Email пользователя
        public string Password { get; set; } // Пароль пользователя
    }
}
