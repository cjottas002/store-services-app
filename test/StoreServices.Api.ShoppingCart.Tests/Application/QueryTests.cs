using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.ShoppingCart.Application;
using StoreServices.Api.ShoppingCart.Model;
using StoreServices.Api.ShoppingCart.Persistence;
using StoreServices.Api.ShoppingCart.RemoteInterface;
using StoreServices.Api.ShoppingCart.RemoteModel;
using Xunit;

namespace StoreServices.Api.ShoppingCart.Tests.Application
{
    public class QueryTests
    {
        private CartContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<CartContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new CartContext(options);
        }

        [Fact]
        public async Task GetCart_ReturnsCartWithProducts()
        {
            // Arrange
            var context = CreateContext("CartQuery_WithProducts");
            var bookId = Guid.NewGuid();
            var session = new CartSession { CreationDate = DateTime.Now };
            context.CartSession.Add(session);
            await context.SaveChangesAsync();
            context.CartSessionDetail.Add(new CartSessionDetail
            {
                CartSessionId = session.CartSessionId,
                CreationDate = DateTime.Now,
                SelectedProduct = bookId.ToString()
            });
            await context.SaveChangesAsync();
            var mockBookService = new Mock<IBooksService>();
            mockBookService
                .Setup(s => s.GetBook(bookId))
                .ReturnsAsync((true, new BookRemote
                {
                    LibraryMaterialId = bookId,
                    Title = "Test Book",
                    PublicationDate = new DateTime(2023, 1, 1)
                }, null));
            var handler = new Query.Handler(context, mockBookService.Object);

            // Act
            var result = await handler.Handle(
                new Query.Execute { CartSessionId = session.CartSessionId },
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(session.CartSessionId, result.CartId);
            Assert.Single(result.Products);
            Assert.Equal("Test Book", result.Products[0].BookTitle);
        }

        [Fact]
        public async Task GetCart_BookServiceFails_ReturnsEmptyProducts()
        {
            // Arrange
            var context = CreateContext("CartQuery_BookFails");
            var bookId = Guid.NewGuid();
            var session = new CartSession { CreationDate = DateTime.Now };
            context.CartSession.Add(session);
            await context.SaveChangesAsync();
            context.CartSessionDetail.Add(new CartSessionDetail
            {
                CartSessionId = session.CartSessionId,
                CreationDate = DateTime.Now,
                SelectedProduct = bookId.ToString()
            });
            await context.SaveChangesAsync();
            var mockBookService = new Mock<IBooksService>();
            mockBookService
                .Setup(s => s.GetBook(bookId))
                .ReturnsAsync((false, null, "Service unavailable"));
            var handler = new Query.Handler(context, mockBookService.Object);

            // Act
            var result = await handler.Handle(
                new Query.Execute { CartSessionId = session.CartSessionId },
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Products);
        }

        [Fact]
        public async Task GetCart_MultipleProducts_ReturnsAll()
        {
            // Arrange
            var context = CreateContext("CartQuery_MultipleProducts");
            var bookId1 = Guid.NewGuid();
            var bookId2 = Guid.NewGuid();
            var session = new CartSession { CreationDate = DateTime.Now };
            context.CartSession.Add(session);
            await context.SaveChangesAsync();
            context.CartSessionDetail.AddRange(
                new CartSessionDetail
                {
                    CartSessionId = session.CartSessionId,
                    CreationDate = DateTime.Now,
                    SelectedProduct = bookId1.ToString()
                },
                new CartSessionDetail
                {
                    CartSessionId = session.CartSessionId,
                    CreationDate = DateTime.Now,
                    SelectedProduct = bookId2.ToString()
                }
            );
            await context.SaveChangesAsync();
            var mockBookService = new Mock<IBooksService>();
            mockBookService
                .Setup(s => s.GetBook(bookId1))
                .ReturnsAsync((true, new BookRemote
                {
                    LibraryMaterialId = bookId1,
                    Title = "Book One",
                    PublicationDate = DateTime.Now
                }, null));
            mockBookService
                .Setup(s => s.GetBook(bookId2))
                .ReturnsAsync((true, new BookRemote
                {
                    LibraryMaterialId = bookId2,
                    Title = "Book Two",
                    PublicationDate = DateTime.Now
                }, null));
            var handler = new Query.Handler(context, mockBookService.Object);

            // Act
            var result = await handler.Handle(
                new Query.Execute { CartSessionId = session.CartSessionId },
                CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Products.Count);
        }

        [Fact]
        public async Task GetCart_NoDetails_ReturnsEmptyProducts()
        {
            // Arrange
            var context = CreateContext("CartQuery_NoDetails");
            var session = new CartSession { CreationDate = DateTime.Now };
            context.CartSession.Add(session);
            await context.SaveChangesAsync();
            var mockBookService = new Mock<IBooksService>();
            var handler = new Query.Handler(context, mockBookService.Object);

            // Act
            var result = await handler.Handle(
                new Query.Execute { CartSessionId = session.CartSessionId },
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Products!);
            mockBookService.Verify(s => s.GetBook(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetCart_PartialBookFailure_ReturnsOnlySuccessful()
        {
            // Arrange
            var context = CreateContext("CartQuery_PartialFail");
            var bookId1 = Guid.NewGuid();
            var bookId2 = Guid.NewGuid();
            var session = new CartSession { CreationDate = DateTime.Now };
            context.CartSession.Add(session);
            await context.SaveChangesAsync();
            context.CartSessionDetail.AddRange(
                new CartSessionDetail
                {
                    CartSessionId = session.CartSessionId,
                    CreationDate = DateTime.Now,
                    SelectedProduct = bookId1.ToString()
                },
                new CartSessionDetail
                {
                    CartSessionId = session.CartSessionId,
                    CreationDate = DateTime.Now,
                    SelectedProduct = bookId2.ToString()
                }
            );
            await context.SaveChangesAsync();
            var mockBookService = new Mock<IBooksService>();
            mockBookService
                .Setup(s => s.GetBook(bookId1))
                .ReturnsAsync((true, new BookRemote
                {
                    LibraryMaterialId = bookId1,
                    Title = "Available Book",
                    PublicationDate = DateTime.Now
                }, null));
            mockBookService
                .Setup(s => s.GetBook(bookId2))
                .ReturnsAsync((false, null, "Not Found"));
            var handler = new Query.Handler(context, mockBookService.Object);

            // Act
            var result = await handler.Handle(
                new Query.Execute { CartSessionId = session.CartSessionId },
                CancellationToken.None);

            // Assert
            Assert.Single(result.Products!);
            Assert.Equal("Available Book", result.Products![0].BookTitle);
        }

        [Fact]
        public async Task GetCart_ReturnsCorrectSessionDate()
        {
            // Arrange
            var context = CreateContext("CartQuery_SessionDate");
            var date = new DateTime(2025, 3, 15);
            var session = new CartSession { CreationDate = date };
            context.CartSession.Add(session);
            await context.SaveChangesAsync();
            var mockBookService = new Mock<IBooksService>();
            var handler = new Query.Handler(context, mockBookService.Object);

            // Act
            var result = await handler.Handle(
                new Query.Execute { CartSessionId = session.CartSessionId },
                CancellationToken.None);

            // Assert
            Assert.Equal(date, result.SessionCreationDate);
        }
    }
}
