using Microsoft.EntityFrameworkCore;
using StoreServices.Api.Author.Model;

namespace StoreServices.Api.Author.Persistence;

public class AuthorContext(DbContextOptions<AuthorContext> options) : DbContext(options)
{
    public DbSet<AuthorBook> AuthorBook { get; set; }
    public DbSet<AcademicDegree> AcademicDegree { get; set; }
}
