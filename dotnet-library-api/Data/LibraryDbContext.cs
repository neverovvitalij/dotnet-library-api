using Microsoft.EntityFrameworkCore;
using dotnet_library_api.Domain.Models;
namespace dotnet_library_api.Data;

public class LibraryDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
        
    }
}
