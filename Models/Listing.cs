using System;
using System.ComponentModel.DataAnnotations;

public class Listing
{
    // Уникальный идентификатор
    public int Id { get; set; }

    // Название (обязательно)
    [Required(ErrorMessage = "Название обязательно.")]
    public string Title { get; set; }

    // Состояние (обязательно)
    [Required(ErrorMessage = "Состояние обязательно.")]
    public string Condition { get; set; }

    // Описание (обязательно)
    [Required(ErrorMessage = "Описание обязательно.")]
    public string Description { get; set; }

    // Категория (обязательно)
    [Required(ErrorMessage = "Категория обязательна.")]
    public string Category { get; set; }

    // Город (обязательно)
    [Required(ErrorMessage = "Город обязателен.")]
    public string City { get; set; }

    // Путь к фотографии (обязательно)
    [Required(ErrorMessage = "Фотография обязательна.")]
    public string PhotoPath { get; set; }

    // Тип объявления (по умолчанию "Нужно")
    [Required(ErrorMessage = "Тип объявления обязателен.")]
    private string _type = "Нужно";
    public string Type
    {
        get => _type;
        set => _type = string.IsNullOrWhiteSpace(value) ? "Нужно" : value;
    }

    // Идентификатор пользователя
    public int UserId { get; set; }

    // Дата и время создания (UTC)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Флаг архивирования
    public bool IsArchived { get; set; } = false;
}
