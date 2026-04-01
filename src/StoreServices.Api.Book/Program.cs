using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreServices.Api.Book.Application;
using StoreServices.Api.Book.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Create>();

builder.Services.AddDbContext<LibraryContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"));
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Create.Handler).Assembly));
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Query.Execute).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();
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
