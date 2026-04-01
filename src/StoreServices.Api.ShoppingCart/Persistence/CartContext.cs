using Microsoft.EntityFrameworkCore;
using StoreServices.Api.ShoppingCart.Model;

namespace StoreServices.Api.ShoppingCart.Persistence;

public class CartContext(DbContextOptions<CartContext> options) : DbContext(options)
{
    public DbSet<CartSession> CartSession { get; set; }
    public DbSet<CartSessionDetail> CartSessionDetail { get; set; }
}
