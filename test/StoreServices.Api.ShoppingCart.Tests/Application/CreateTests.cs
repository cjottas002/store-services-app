using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.ShoppingCart.Application;
using StoreServices.Api.ShoppingCart.Persistence;
using Xunit;

namespace StoreServices.Api.ShoppingCart.Tests.Application
{
    public class CreateTests
    {
        private CartContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<CartContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new CartContext(options);
        }

        [Fact]
        public async Task CreateCart_InsertsSessionIntoContext()
        {
            // Arrange
            var context = CreateContext("Cart_Create");
            var request = new Create.Execute
            {
                SessionCreationDate = DateTime.Now,
                ProductList = new List<string> { Guid.NewGuid().ToString() }
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(context.CartSession.Any());
        }

        [Fact]
        public async Task CreateCart_InsertsDetailsForEachProduct()
        {
            // Arrange
            var context = CreateContext("Cart_Create_Details");
            var products = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
            var request = new Create.Execute
            {
                SessionCreationDate = DateTime.Now,
                ProductList = products
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(3, context.CartSessionDetail.Count());
        }

        [Fact]
        public async Task CreateCart_DetailLinksToSession()
        {
            // Arrange
            var context = CreateContext("Cart_Create_Link");
            var productId = Guid.NewGuid().ToString();
            var request = new Create.Execute
            {
                SessionCreationDate = DateTime.Now,
                ProductList = new List<string> { productId }
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var session = context.CartSession.First();
            var detail = context.CartSessionDetail.First();
            Assert.Equal(session.CartSessionId, detail.CartSessionId);
            Assert.Equal(productId, detail.SelectedProduct);
        }

        [Fact]
        public async Task CreateCart_PersistsCreationDate()
        {
            // Arrange
            var context = CreateContext("Cart_Create_Date");
            var date = new DateTime(2025, 6, 15);
            var request = new Create.Execute
            {
                SessionCreationDate = date,
                ProductList = new List<string> { Guid.NewGuid().ToString() }
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var session = context.CartSession.First();
            Assert.Equal(date, session.CreationDate);
        }

        [Fact]
        public async Task CreateCart_MultipleProducts_AllDetailsHaveCorrectSessionId()
        {
            // Arrange
            var context = CreateContext("Cart_Create_AllLinked");
            var products = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
            var request = new Create.Execute
            {
                SessionCreationDate = DateTime.Now,
                ProductList = products
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var session = context.CartSession.First();
            var details = context.CartSessionDetail.ToList();
            Assert.All(details, d => Assert.Equal(session.CartSessionId, d.CartSessionId));
        }

        [Fact]
        public async Task CreateCart_EachDetailHasCreationDate()
        {
            // Arrange
            var context = CreateContext("Cart_Create_DetailDates");
            var request = new Create.Execute
            {
                SessionCreationDate = DateTime.Now,
                ProductList = new List<string>
                {
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString()
                }
            };
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var details = context.CartSessionDetail.ToList();
            Assert.All(details, d => Assert.NotNull(d.CreationDate));
        }

        [Fact]
        public async Task CreateCart_MultipleSessions_IndependentIds()
        {
            // Arrange
            var context = CreateContext("Cart_Create_MultipleSessions");
            var handler = new Create.Handler(context);

            // Act
            await handler.Handle(new Create.Execute
            {
                SessionCreationDate = DateTime.Now,
                ProductList = new List<string> { Guid.NewGuid().ToString() }
            }, CancellationToken.None);

            await handler.Handle(new Create.Execute
            {
                SessionCreationDate = DateTime.Now,
                ProductList = new List<string> { Guid.NewGuid().ToString() }
            }, CancellationToken.None);

            // Assert
            var sessions = context.CartSession.ToList();
            Assert.Equal(2, sessions.Count);
            Assert.NotEqual(sessions[0].CartSessionId, sessions[1].CartSessionId);
        }
    }
}
