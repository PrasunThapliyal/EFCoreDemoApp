namespace EFCoreDemoApp.Models
{
    public class BookStore
    {
        public int BookStoreId { get; set; }
        public string? Name { get; set; }

        // The navigation property must be virtual for lazy loading
        public virtual ICollection<Book> Books { get; set; } = [];
        public virtual ICollection<Author> Authors { get; set; } = [];

    }
}
