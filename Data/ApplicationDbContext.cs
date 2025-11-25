using Microsoft.EntityFrameworkCore;
using BookApi.Models;

namespace BookApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Quote> Quotes { get; set; }
}
