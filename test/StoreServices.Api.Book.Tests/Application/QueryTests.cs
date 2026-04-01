using AutoMapper;
using GenFu;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.Book.Application;
using StoreServices.Api.Book.Model;
using StoreServices.Api.Book.Persistence;
using StoreServices.Api.Book.Tests.Helpers;
using Xunit;

namespace StoreServices.Api.Book.Tests.Application
{
    public class QueryTests
    {
        private LibraryContext CreateSeededContext(string dbName, int count = 30)
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new LibraryContext(options);

            A.Configure<LibraryMaterial>()
                .Fill(x => x.Title).AsArticleTitle()
                .Fill(x => x.LibraryMaterialId, () => Guid.NewGuid());

            var list = A.ListOf<LibraryMaterial>(count);
            context.LibraryMaterial.AddRange(list);
            context.SaveChanges();

            return context;
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
        public async Task GetBooks_ReturnsAllBooks()
        {
            // Arrange
            var context = CreateSeededContext("Query_GetBooks");
            var mapper = CreateMapper();
            var handler = new Query.Handler(context, mapper);

            // Act
            var list = await handler.Handle(new Query.Execute(), CancellationToken.None);

            // Assert
            Assert.True(list.Any());
            Assert.Equal(30, list.Count);
        }

        [Fact]
        public async Task GetBooks_ReturnsDtoWithCorrectProperties()
        {
            // Arrange
            var context = CreateSeededContext("Query_DtoProperties", 5);
            var mapper = CreateMapper();
            var handler = new Query.Handler(context, mapper);

            // Act
            var list = await handler.Handle(new Query.Execute(), CancellationToken.None);

            // Assert
            Assert.All(list, dto =>
            {
                Assert.NotNull(dto.LibraryMaterialId);
                Assert.False(string.IsNullOrEmpty(dto.Title));
            });
        }

        [Fact]
        public async Task GetBooks_EmptyDb_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "Query_EmptyDb")
                .Options;
            var context = new LibraryContext(options);
            var mapper = CreateMapper();
            var handler = new Query.Handler(context, mapper);

            // Act
            var list = await handler.Handle(new Query.Execute(), CancellationToken.None);

            // Assert
            Assert.Empty(list);
        }
    }
}
