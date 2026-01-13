namespace EFCoreDemoApp.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string? Title { get; set; }
        public int BookStoreId { get; set; }
        public int AuthorId { get; set; }

        // The navigation property must be virtual for lazy loading
        public virtual Author? Author { get; set; }
    }
}
