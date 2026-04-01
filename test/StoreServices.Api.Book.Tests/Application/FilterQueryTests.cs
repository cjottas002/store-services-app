using AutoMapper;
using GenFu;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.Book.Application;
using StoreServices.Api.Book.Model;
using StoreServices.Api.Book.Persistence;
using StoreServices.Api.Book.Tests.Helpers;
using Xunit;

namespace StoreServices.Api.Book.Tests.Application
{
    public class FilterQueryTests
    {
        private (LibraryContext context, Guid knownId) CreateSeededContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new LibraryContext(options);
            var knownId = Guid.NewGuid();

            A.Configure<LibraryMaterial>()
                .Fill(x => x.Title).AsArticleTitle()
                .Fill(x => x.LibraryMaterialId, () => Guid.NewGuid());

            var list = A.ListOf<LibraryMaterial>(30);
            list[0].LibraryMaterialId = knownId;

            context.LibraryMaterial.AddRange(list);
            context.SaveChanges();

            return (context, knownId);
        }

        private IMapper CreateMapper()
        {
            var mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TestMappingProfile());
            }, NullLoggerFactory.Instance);
            return mapConfig.CreateMapper();
        }

        [Fact]
        public async Task GetBookById_ReturnsCorrectBook()
        {
            // Arrange
            var (context, knownId) = CreateSeededContext("Filter_GetById");
            var mapper = CreateMapper();
            var handler = new FilterQuery.Handler(context, mapper);
            var request = new FilterQuery.SingleBook { BookId = knownId };

            // Act
            var book = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(book);
            Assert.Equal(knownId, book.LibraryMaterialId);
        }

        [Fact]
        public async Task GetBookById_NonExistent_ThrowsException()
        {
            // Arrange
            var (context, _) = CreateSeededContext("Filter_NonExistent");
            var mapper = CreateMapper();
            var handler = new FilterQuery.Handler(context, mapper);
            var request = new FilterQuery.SingleBook { BookId = Guid.NewGuid() };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task GetBookById_ReturnsMappedDto()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "Filter_MappedDto")
                .Options;
            var context = new LibraryContext(options);
            var bookId = Guid.NewGuid();
            context.LibraryMaterial.Add(new LibraryMaterial
            {
                LibraryMaterialId = bookId,
                Title = "Test Book",
                PublicationDate = new DateTime(2023, 6, 15),
                AuthorBook = Guid.NewGuid()
            });
            context.SaveChanges();
            var mapper = CreateMapper();
            var handler = new FilterQuery.Handler(context, mapper);

            // Act
            var result = await handler.Handle(new FilterQuery.SingleBook { BookId = bookId }, CancellationToken.None);

            // Assert
            Assert.Equal("Test Book", result.Title);
            Assert.Equal(new DateTime(2023, 6, 15), result.PublicationDate);
        }
    }
}
