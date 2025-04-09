using Microsoft.EntityFrameworkCore;
using BarterAppAPI.Data;



var builder = WebApplication.CreateBuilder(args);

// Подключаем базу данных PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавляем контроллеры и Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Включаем Swagger в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Логирование запросов
app.Use(async (context, next) =>
{
    Console.WriteLine($"[{DateTime.Now}] {context.Request.Method} {context.Request.Path}");
    await next();
});

// Глобальная обработка ошибок
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\":\"Internal Server Error\"}");
    });
});

// Включаем HTTPS, CORS, авторизацию и статические файлы
app.UseHttpsRedirection();
app.UseCors(builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());
app.UseAuthorization();
app.UseStaticFiles();

// Маршрутизация контроллеров и базовый эндпоинт
app.MapControllers();
app.MapGet("/", () => "API is running!");

// Запускаем приложение
app.Run();
public partial class Program { }
