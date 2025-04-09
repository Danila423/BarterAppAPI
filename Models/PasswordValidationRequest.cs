namespace BarterAppAPI.Models
{
    // Класс запроса для валидации пароля
    public class PasswordValidationRequest
    {
        // Идентификатор пользователя
        public int UserId { get; set; }

        // Пароль для проверки
        public string Password { get; set; }
    }
}
