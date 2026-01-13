using EFCoreDemoApp.DatabaseInfra;
using EFCoreDemoApp.Models;
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

        [HttpPost("seedData")]
        public async Task<IActionResult> SeedData()
        {
            // Seed some initial data

            _logger.LogInformation($"SeedData: Seeding some data ..");

            var authors = new List<Author> 
            {
                new Author { AuthorId = 1, Name = "J.R.R. Tolkien" },
                new Author { AuthorId = 2, Name = "Kernighan, Ritchie" }
            };
            var books = new List<Book>()
            {
                new Book { BookId = 1, Title = "The Hobbit", AuthorId = 1 },
                new Book { BookId = 2, Title = "The Lord of the Rings", AuthorId = 1 },
                new Book { BookId = 3, Title = "The C Programming Language", AuthorId = 2 }
            };

            if (_dbContext != null)
            {
                if (_dbContext.Authors != null)
                {
                    foreach (var author in authors)
                    {
                        _dbContext.Authors.Add(author);
                    }
                }

                if (_dbContext.Books != null)
                {
                    foreach (var book in books)
                    {
                        _dbContext.Books.Add(book);
                    }
                }

                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }


        [HttpGet("Books")]
        public async Task<IActionResult> GetAllBooks(bool readLinkedAuthors = false)
        {
            if (_dbContext.Books != null)
            {
                var books = await _dbContext.Books.ToListAsync();

                if (readLinkedAuthors)
                {
                    foreach (var book in books)
                    {
                        var author = book.Author;
                        _logger.LogInformation($"Book: {book.Title} written by {author?.Name}");
                    }
                }

                //return Ok(books);
            }

            return Ok();
        }

        [HttpGet("Authors")]
        public async Task<IActionResult> GetAllAuthors()
        {
            if (_dbContext.Authors != null)
            {
                var authors = await _dbContext.Authors.ToListAsync();
                //return Ok(authors);
            }

            return Ok();
        }

        [HttpPost("Book")]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            if (_dbContext.Books != null)
            {
                var books = _dbContext.Books;
                books.Add(book);

                await _dbContext.SaveChangesAsync();

                return Ok(book);
            }

            return BadRequest();
        }
    }
}
