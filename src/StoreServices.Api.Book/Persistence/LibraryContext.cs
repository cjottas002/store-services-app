using Microsoft.EntityFrameworkCore;
using StoreServices.Api.Book.Model;

namespace StoreServices.Api.Book.Persistence;

public class LibraryContext(DbContextOptions<LibraryContext> options) : DbContext(options)
{
    public virtual DbSet<LibraryMaterial> LibraryMaterial { get; set; }
}
