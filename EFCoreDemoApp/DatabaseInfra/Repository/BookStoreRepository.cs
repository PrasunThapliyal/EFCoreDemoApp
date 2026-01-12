using System.ComponentModel;

namespace EFCoreDemoApp.DatabaseInfra.Repository
{
    public class BookStoreRepository
    {
        private readonly ILogger<BookStoreRepository> _logger;
        private readonly AppDbContext _dbContext;

        public BookStoreRepository(ILogger<BookStoreRepository> logger, AppDbContext dbContext)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

    }
}
