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
    public class FilterQueryTests
    {
        private (AuthorContext context, string knownGuid) CreateSeededContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AuthorContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AuthorContext(options);
            var knownGuid = Guid.NewGuid().ToString();

            context.AuthorBook.Add(new AuthorBook
            {
                Name = "Known",
                LastName = "Author",
                AuthorBookGuid = knownGuid,
                BirthDate = new DateTime(1980, 1, 1)
            });

            for (int i = 1; i < 10; i++)
            {
                context.AuthorBook.Add(new AuthorBook
                {
                    Name = $"Author{i}",
                    LastName = $"Last{i}",
                    AuthorBookGuid = Guid.NewGuid().ToString()
                });
            }
            context.SaveChanges();

            return (context, knownGuid);
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
        public async Task GetAuthorByGuid_ReturnsCorrectAuthor()
        {
            // Arrange
            var (context, knownGuid) = CreateSeededContext("AuthorFilter_ByGuid");
            var mapper = CreateMapper();
            var handler = new FilterQuery.Handler(context, mapper);
            var request = new FilterQuery.SingleAuthor { AuthorGuid = knownGuid };

            // Act
            var author = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(author);
            Assert.Equal(knownGuid, author.AuthorBookGuid);
            Assert.Equal("Known", author.Name);
            Assert.Equal("Author", author.LastName);
        }

        [Fact]
        public async Task GetAuthorByGuid_NonExistent_ThrowsException()
        {
            // Arrange
            var (context, _) = CreateSeededContext("AuthorFilter_NonExistent");
            var mapper = CreateMapper();
            var handler = new FilterQuery.Handler(context, mapper);
            var request = new FilterQuery.SingleAuthor { AuthorGuid = Guid.NewGuid().ToString() };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                handler.Handle(request, CancellationToken.None));
        }
    }
}
