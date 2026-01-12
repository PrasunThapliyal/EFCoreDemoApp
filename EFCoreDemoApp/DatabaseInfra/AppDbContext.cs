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
            //// Seed some initial data
            //modelBuilder.Entity<Author>().HasData(new Author { AuthorId = 1, Name = "J.R.R. Tolkien" });
            //modelBuilder.Entity<Book>().HasData(
            //    new Book { BookId = 1, Title = "The Hobbit", AuthorId = 1 },
            //    new Book { BookId = 2, Title = "The Lord of the Rings", AuthorId = 1 }
            //);
        }
    }
}
