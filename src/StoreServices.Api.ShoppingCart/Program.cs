using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreServices.Api.ShoppingCart.Application;
using StoreServices.Api.ShoppingCart.Persistence;
using StoreServices.Api.ShoppingCart.RemoteInterface;
using StoreServices.Api.ShoppingCart.RemoteService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IBooksService, BooksService>();

builder.Services.AddDbContext<CartContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("DatabaseConnection")!);
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Create.Handler).Assembly));

builder.Services.AddHttpClient("Books", config =>
{
    config.BaseAddress = new Uri(builder.Configuration["Services:Books"]!);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<CartContext>();
    var retries = 10;
    while (retries > 0)
    {
        try
        {
            context.Database.EnsureCreated();
            break;
        }
        catch
        {
            retries--;
            Thread.Sleep(3000);
        }
    }
}

app.UseAuthorization();
app.MapControllers();

app.Run();
