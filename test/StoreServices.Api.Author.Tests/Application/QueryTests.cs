using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.Author.Application;
using StoreServices.Api.Author.Model;
using StoreServices.Api.Author.Persistence;
using StoreServices.Api.Author.Tests.Helpers;
using Xunit;

namespace StoreServices.Api.Author.Tests.Application
{
    public class QueryTests
    {
        private AuthorContext CreateSeededContext(string dbName, int count = 20)
        {
            var options = new DbContextOptionsBuilder<AuthorContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AuthorContext(options);

            for (int i = 0; i < count; i++)
            {
                context.AuthorBook.Add(new AuthorBook
                {
                    Name = $"Author{i}",
                    LastName = $"Last{i}",
                    AuthorBookGuid = Guid.NewGuid().ToString(),
                    BirthDate = new DateTime(1960 + i % 40, 1, 1)
                });
            }
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
        public async Task GetAuthors_ReturnsAllAuthors()
        {
            // Arrange
            var context = CreateSeededContext("AuthorQuery_GetAll");
            var mapper = CreateMapper();
            var handler = new Query.Handler(context, mapper);

            // Act
            var list = await handler.Handle(new Query.AuthorList(), CancellationToken.None);

            // Assert
            Assert.Equal(20, list.Count);
        }

        [Fact]
        public async Task GetAuthors_ReturnsDtoWithCorrectProperties()
        {
            // Arrange
            var context = CreateSeededContext("AuthorQuery_DtoProps", 5);
            var mapper = CreateMapper();
            var handler = new Query.Handler(context, mapper);

            // Act
            var list = await handler.Handle(new Query.AuthorList(), CancellationToken.None);

            // Assert
            Assert.All(list, dto =>
            {
                Assert.False(string.IsNullOrEmpty(dto.Name));
                Assert.False(string.IsNullOrEmpty(dto.LastName));
                Assert.False(string.IsNullOrEmpty(dto.AuthorBookGuid));
            });
        }

        [Fact]
        public async Task GetAuthors_EmptyDb_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AuthorContext>()
                .UseInMemoryDatabase(databaseName: "AuthorQuery_EmptyDb")
                .Options;
            var context = new AuthorContext(options);
            var mapper = CreateMapper();
            var handler = new Query.Handler(context, mapper);

            // Act
            var list = await handler.Handle(new Query.AuthorList(), CancellationToken.None);

            // Assert
            Assert.Empty(list);
        }
    }
}
