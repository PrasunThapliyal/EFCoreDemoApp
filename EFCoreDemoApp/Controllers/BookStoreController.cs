using EFCoreDemoApp.DatabaseInfra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreDemoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookStoreController : ControllerBase
    {
        private readonly ILogger<BookStoreController> _logger;
        private readonly AppDbContext _dbContext;

        public BookStoreController(ILogger<BookStoreController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet("Books")]
        public async Task<IActionResult> GetAllBooks()
        {
            if (_dbContext.Books != null)
            {
                var books = await _dbContext.Books.ToListAsync();
                return Ok(books);
            }

            return Ok();
        }
    }
}
