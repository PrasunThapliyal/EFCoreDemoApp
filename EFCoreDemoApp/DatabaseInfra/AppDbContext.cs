using EFCoreDemoApp.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace EFCoreDemoApp.DatabaseInfra
{
    public class AppDbContext : DbContext
    {
        private readonly ILogger<AppDbContext> _logger;

        public DbSet<BookStore>? BookStores { get; set; }
        public DbSet<Author>? Authors { get; set; }
        public DbSet<Book>? Books { get; set; }

        public AppDbContext(ILogger<AppDbContext> logger, DbContextOptions<AppDbContext> options)
            : base(options)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _logger.LogInformation($"AppDbContext: OnConfiguring ..");

            // Configure SQLite and enable lazy loading proxies
            optionsBuilder.UseLazyLoadingProxies()
                          .UseSqlite("Data Source=bookstore.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
