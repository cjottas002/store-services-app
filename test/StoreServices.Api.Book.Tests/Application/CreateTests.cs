using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.Book.Application;
using StoreServices.Api.Book.Persistence;
using Xunit;

namespace StoreServices.Api.Book.Tests.Application
{
    public class CreateTests
    {
        [Fact]
        public async Task SaveBook_InsertsBookIntoContext()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "SaveBook")
                .Options;
            var context = new LibraryContext(options);
            var request = new Create.Execute
            {
                Title = "Microservice Book",
                AuthorBook = Guid.NewGuid(),
                PublicationDate = DateTime.Now
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(context.LibraryMaterial.Any());
        }

        [Fact]
        public async Task SaveBook_PersistsCorrectTitle()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "SaveBook_Title")
                .Options;
            var context = new LibraryContext(options);
            var request = new Create.Execute
            {
                Title = "Clean Architecture",
                AuthorBook = Guid.NewGuid(),
                PublicationDate = new DateTime(2020, 1, 1)
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var saved = context.LibraryMaterial.First();
            Assert.Equal("Clean Architecture", saved.Title);
            Assert.Equal(request.AuthorBook, saved.AuthorBook);
        }

        [Fact]
        public void Validation_RejectsEmptyTitle()
        {
            // Arrange
            var validator = new Create.ExecuteValidation();
            var request = new Create.Execute
            {
                Title = "",
                AuthorBook = Guid.NewGuid(),
                PublicationDate = DateTime.Now
            };

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validation_RejectsNullPublicationDate()
        {
            // Arrange
            var validator = new Create.ExecuteValidation();
            var request = new Create.Execute
            {
                Title = "Some Title",
                AuthorBook = Guid.NewGuid(),
                PublicationDate = null
            };

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
                Title = "Valid Book",
                AuthorBook = Guid.NewGuid(),
                PublicationDate = DateTime.Now
            };

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validation_RejectsNullAuthorBook()
        {
            // Arrange
            var validator = new Create.ExecuteValidation();
            var request = new Create.Execute
            {
                Title = "Some Title",
                AuthorBook = null,
                PublicationDate = DateTime.Now
            };

            // Act
            var result = validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task SaveBook_GeneratesLibraryMaterialId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "SaveBook_GeneratesId")
                .Options;
            var context = new LibraryContext(options);
            var request = new Create.Execute
            {
                Title = "New Book",
                AuthorBook = Guid.NewGuid(),
                PublicationDate = DateTime.Now
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var saved = context.LibraryMaterial.First();
            Assert.NotNull(saved.LibraryMaterialId);
        }

        [Fact]
        public async Task SaveBook_MultipleBooks_AllPersisted()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "SaveBook_Multiple")
                .Options;
            var context = new LibraryContext(options);
            var handler = new Create.Handler(context);

            // Act
            for (int i = 0; i < 5; i++)
            {
                await handler.Handle(new Create.Execute
                {
                    Title = $"Book {i}",
                    AuthorBook = Guid.NewGuid(),
                    PublicationDate = DateTime.Now
                }, CancellationToken.None);
            }

            // Assert
            Assert.Equal(5, context.LibraryMaterial.Count());
        }
    }
}
