using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.Author.Application;
using StoreServices.Api.Author.Persistence;
using Xunit;

namespace StoreServices.Api.Author.Tests.Application
{
    public class CreateTests
    {
        private AuthorContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AuthorContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AuthorContext(options);
        }

        [Fact]
        public async Task SaveAuthor_InsertsAuthorIntoContext()
        {
            // Arrange
            var context = CreateContext("Author_Save");
            var request = new Create.Execute
            {
                Name = "Robert",
                LastName = "Martin",
                BirthDate = new DateTime(1952, 12, 5)
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(context.AuthorBook.Any());
        }

        [Fact]
        public async Task SaveAuthor_PersistsCorrectData()
        {
            // Arrange
            var context = CreateContext("Author_Save_Data");
            var request = new Create.Execute
            {
                Name = "Martin",
                LastName = "Fowler",
                BirthDate = new DateTime(1963, 12, 18)
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var saved = context.AuthorBook.First();
            Assert.Equal("Martin", saved.Name);
            Assert.Equal("Fowler", saved.LastName);
            Assert.NotNull(saved.AuthorBookGuid);
        }

        [Fact]
        public async Task SaveAuthor_GeneratesGuid()
        {
            // Arrange
            var context = CreateContext("Author_Save_Guid");
            var request = new Create.Execute
            {
                Name = "Eric",
                LastName = "Evans",
                BirthDate = new DateTime(1960, 1, 1)
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var saved = context.AuthorBook.First();
            Assert.False(string.IsNullOrEmpty(saved.AuthorBookGuid));
        }

        [Fact]
        public void Validation_RejectsEmptyName()
        {
            // Arrange
            var validator = new Create.ExecuteValidation();
            var request = new Create.Execute { Name = "", LastName = "Martin" };

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validation_RejectsEmptyLastName()
        {
            // Arrange
            var validator = new Create.ExecuteValidation();
            var request = new Create.Execute { Name = "Robert", LastName = "" };

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validation_AcceptsValidRequest()
        {
            // Arrange
            var validator = new Create.ExecuteValidation();
            var request = new Create.Execute
            {
                Name = "Robert",
                LastName = "Martin",
                BirthDate = new DateTime(1952, 12, 5)
            };

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
