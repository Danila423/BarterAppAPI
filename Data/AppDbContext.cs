using Microsoft.EntityFrameworkCore;
using BarterAppAPI.Models;

namespace BarterAppAPI.Data
{
    // Класс контекста базы данных, который наследуется от DbContext.
    public class AppDbContext : DbContext
    {
        // DbSet для пользователей
        public DbSet<User> Users { get; set; }

        // DbSet для объявлений
        public DbSet<Listing> Listings { get; set; }

        // DbSet для избранных объявлений
        public DbSet<Favorite> Favorites { get; set; }

        // Конструктор
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
