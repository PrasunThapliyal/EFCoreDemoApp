using EFCoreDemoApp.Models;
using Microsoft.EntityFrameworkCore;

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
            _logger.LogInformation($"AppDbContext: OnConfiguring **** ..");

            if (!optionsBuilder.IsConfigured)
            {
                _logger.LogInformation($"AppDbContext: OnConfiguring: **** Configuring OptionsBuilder to use Postgres with Snake Case convention ..");


                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                var configuration = builder.Build();
                var connStr = configuration.GetConnectionString("PostgresConnection");

                // Configure SQLite and enable lazy loading proxies
                optionsBuilder
                //.LogTo(Console.WriteLine) // Log to the console (or any Action<string>)
                //.EnableSensitiveDataLogging() // **Crucial**: Enables logging of parameter values
                .UseLazyLoadingProxies()
                //.UseSqlite("Data Source=bookstore.db");
                .UseNpgsql(connStr)
                .UseSnakeCaseNamingConvention();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _logger.LogInformation($"AppDbContext: OnModelCreating ..");

            //modelBuilder.HasDefaultSchema("efcoredemoapp");

            //modelBuilder.Entity<Book>().ToTable("books");
            //modelBuilder.Entity<Author>().ToTable("authors");
            //modelBuilder.Entity<BookStore>().ToTable("bookstores");
        }
    }
}
