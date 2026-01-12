namespace EFCoreDemoApp.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string? Name { get; set; }
        // The navigation property must be virtual for lazy loading
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    }
}
