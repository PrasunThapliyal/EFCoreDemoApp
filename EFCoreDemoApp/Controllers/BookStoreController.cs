using EFCoreDemoApp.DatabaseInfra;
using EFCoreDemoApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            var bookStore = new BookStore
            {
                BookStoreId = 1,
                Name = "The Greatest BookStore",
                Authors = authors,
                Books = books
            };


            if (_dbContext != null)
            {
                if (_dbContext.BookStores != null)
                {
                    _dbContext.BookStores.Add(bookStore);
                }

                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }


        [HttpPost("alternateSeeding")]
        public async Task<IActionResult> AlternateSeeding()
        {
            // Seed some initial data

            _logger.LogInformation($"AlternateSeeding: Seeding some data ..");

            var bookStore = new BookStore
            {
                Name = "The Greatest BookStore"
            };

            var author1 = new Author { Name = "J.R.R. Tolkien" };
            var author2 = new Author { Name = "Kernighan, Ritchie" };

            bookStore.Authors.Add(author1);
            bookStore.Authors.Add(author2);

            var book1 = new Book { Title = "The Hobbit", Author = author1 };
            var book2 = new Book { Title = "The Lord of the Rings", Author = author1 };
            var book3 = new Book { Title = "The C Programming Language", Author = author2 };

            bookStore.Books.Add(book1);
            bookStore.Books.Add(book2);
            bookStore.Books.Add(book3);

            if (_dbContext != null)
            {
                if (_dbContext.BookStores != null)
                {
                    _dbContext.BookStores.Add(bookStore);
                }

                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }


        [HttpGet("Books")]
        public async Task<IActionResult> GetAllBooks(int bookStoreId)
        {
            if (_dbContext.BookStores != null)
            {
                var bookStore = await _dbContext.BookStores.Where(p => p.BookStoreId == bookStoreId).FirstOrDefaultAsync();

                var books = bookStore?.Books;

                return Ok(books);
            }

            return Ok();
        }


        [HttpGet("BookTitles")]
        public async Task<IActionResult> GetAllBookTitles(int bookStoreId)
        {
            if (_dbContext.BookStores != null)
            {
                var bookStore = await _dbContext.BookStores.Where(p => p.BookStoreId == bookStoreId).FirstOrDefaultAsync();

                var bookTitles = bookStore?.Books.Select(p => p.Title);
                return Ok(bookTitles);
            }

            return Ok();
        }


        [HttpGet("EagerLoadBooksAndAuthors")]
        public async Task<IActionResult> EagerLoadBooksAndAuthors(int bookStoreId)
        {
            if (_dbContext.BookStores != null)
            {
                var bookStore = await _dbContext.BookStores.Where(p => p.BookStoreId == bookStoreId)
                    .Include(bstore => bstore.Authors)
                    .ThenInclude(author => author.Books)
                    .Include(bstore => bstore.Books)
                    .FirstOrDefaultAsync();

                _logger.LogInformation("EagerLoadBooksAndAuthors: Eager loaded Books and Authors using left join on BookStore");

                //var CProgrammingBook = bookStore?.Authors?.Where(a => a.Name?.Contains("Ritchie") ?? false).FirstOrDefault()?.Books?.FirstOrDefault()?.Title;
                //_logger.LogInformation($"C: {CProgrammingBook}");

                _logger.LogInformation("Collecting Authors ..");
                var authors = bookStore?.Authors.Select(p => new { Name = p.Name, BookTitles = p.Books.Select(q => q.Title).ToList() });

                if (authors != null)
                {
                    foreach (var author in authors)
                    {
                        _logger.LogInformation($"Author: {author.Name}, Books: {string.Join(", ", author.BookTitles)}");
                    }
                }

                //_logger.LogInformation($"Authors: {Newtonsoft.Json.JsonConvert.SerializeObject(authors)}");


                _logger.LogInformation("Collecting Books ..");
                var books = bookStore?.Books.Select(p => new { p.Title, p.Author?.Name });

                _logger.LogInformation($"Books: {Newtonsoft.Json.JsonConvert.SerializeObject(books)}");

                _logger.LogInformation("Retrieved collection of all books along with their Author Name");

                return Ok(books);
            }

            return Ok();
        }


        [HttpGet("Test")]
        public async Task<IActionResult> Test(int bookStoreId = 1)
        {
            if (_dbContext.BookStores != null)
            {
                var bookStore = await _dbContext.BookStores.Where(p => p.BookStoreId == bookStoreId)
                    .FirstOrDefaultAsync();
                /*
                      SELECT "b"."BookStoreId", "b"."Name"
                      FROM "BookStores" AS "b"
                      WHERE "b"."BookStoreId" = @__bookStoreId_0
                      LIMIT 1
                 * 
                 * */

                var authors = bookStore?.Authors.ToList();
                /*
                      SELECT "a"."AuthorId", "a"."BookStoreId", "a"."Name"
                      FROM "Authors" AS "a"
                      WHERE "a"."BookStoreId" = @__p_0
                 * 
                 * */

                var books = bookStore?.Books.ToList();
                /*
                      SELECT "b"."BookId", "b"."AuthorId", "b"."BookStoreId", "b"."Title"
                      FROM "Books" AS "b"
                      WHERE "b"."BookStoreId" = @__p_0
                 * 
                 * */

                /*
                 * This seems to be a missed opportunity by EF Core
                 * We've already read all Authors and Books
                 * At this point, EF Core would have known all entity IDs as well as Foreign Key IDs
                 * Yet, we have to make more queries to establish BookStore.Author.Books relationship
                 * And unless we do explicit eager loading, it still leads to N+1 for BookStore.Author.Books
                 * 
                 * */


                if (_dbContext.Authors != null)
                {
                    var authorsBooks = await _dbContext.Authors.Include(a => a.Books).ToListAsync();
                    /*
                          SELECT "a"."AuthorId", "a"."BookStoreId", "a"."Name", "b"."BookId", "b"."AuthorId", "b"."BookStoreId", "b"."Title"
                          FROM "Authors" AS "a"
                          LEFT JOIN "Books" AS "b" ON "a"."AuthorId" = "b"."AuthorId"
                          ORDER BY "a"."AuthorId"
                     * 
                     * */
                }

                if (authors != null)
                {
                    foreach (var author in authors)
                    {
                        _logger.LogInformation($"Author: {author.Name}, Books: {string.Join(", ", author.Books.Select(p => p.Title))}");
                    }
                }

                var booksAndAuthors = books?.Select(p => new { p.Title, p.Author?.Name });

                _logger.LogInformation($"Books: {Newtonsoft.Json.JsonConvert.SerializeObject(booksAndAuthors)}");

                return Ok(books);
            }

            return Ok();
        }


        [HttpGet("Authors")]
        public async Task<IActionResult> GetAllAuthors(int bookStoreId)
        {
            if (_dbContext.BookStores != null)
            {
                var bookStore = await _dbContext.BookStores.Where(p => p.BookStoreId == bookStoreId).FirstOrDefaultAsync();

                var authors = bookStore?.Authors;

                return Ok(authors);
            }

            return Ok();
        }

        [HttpPost("Book")]
        public async Task<IActionResult> CreateBook(int bookStoreId, string bookTitle, string authorName)
        {
            if (_dbContext.BookStores != null)
            {
                var bookStore = await _dbContext.BookStores.Where(p => p.BookStoreId == bookStoreId).FirstOrDefaultAsync();

                var newAuthor = new Author
                {
                    Name = authorName, //"Dr Be"
                };
                bookStore?.Authors.Add(newAuthor);

                var newBook = new Book
                {
                    Title = bookTitle, //"The Drunken Master",
                    Author = newAuthor
                };
                bookStore?.Books.Add(newBook);

                await _dbContext.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();
        }
    }
}
