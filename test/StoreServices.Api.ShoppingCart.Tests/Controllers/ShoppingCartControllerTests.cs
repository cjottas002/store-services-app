using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.ShoppingCart.Application;
using StoreServices.Api.ShoppingCart.Controllers;
using Xunit;

namespace StoreServices.Api.ShoppingCart.Tests.Controllers;

public class ShoppingCartControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ShoppingCartController _controller;

    public ShoppingCartControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ShoppingCartController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsOkResult()
    {
        // Arrange
        var request = new Create.Execute
        {
            SessionCreationDate = DateTime.Now,
            ProductList = new List<string> { Guid.NewGuid().ToString() }
        };
        _mediatorMock
            .Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(request);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Create_SendsCommandToMediator()
    {
        // Arrange
        var request = new Create.Execute
        {
            SessionCreationDate = DateTime.Now,
            ProductList = new List<string> { Guid.NewGuid().ToString() }
        };
        _mediatorMock
            .Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.Create(request);

        // Assert
        _mediatorMock.Verify(m => m.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCart_ReturnsCartFromMediator()
    {
        // Arrange
        var expected = new CartDto
        {
            CartId = 1,
            SessionCreationDate = DateTime.Now,
            Products = new List<CartDetailDto>
            {
                new() { BookId = Guid.NewGuid(), BookTitle = "Test Book" }
            }
        };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<Query.Execute>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetCart(1);

        // Assert
        Assert.Equal(1, result.Value!.CartId);
        Assert.Single(result.Value.Products!);
    }

    [Fact]
    public async Task GetCart_MediatorThrows_PropagatesException()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<Query.Execute>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Cart not found"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.GetCart(999));
    }
}
