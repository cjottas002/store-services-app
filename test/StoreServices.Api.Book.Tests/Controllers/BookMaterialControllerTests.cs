using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.Book.Application;
using StoreServices.Api.Book.Controllers;
using Xunit;

namespace StoreServices.Api.Book.Tests.Controllers;

public class BookMaterialControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly BookMaterialController _controller;

    public BookMaterialControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new BookMaterialController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsOkResult()
    {
        // Arrange
        var request = new Create.Execute
        {
            Title = "Test Book",
            AuthorBook = Guid.NewGuid(),
            PublicationDate = DateTime.Now
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
            Title = "Test Book",
            AuthorBook = Guid.NewGuid(),
            PublicationDate = DateTime.Now
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
    public async Task GetBooks_ReturnsListFromMediator()
    {
        // Arrange
        var expected = new List<BookMaterialDto>
        {
            new() { LibraryMaterialId = Guid.NewGuid(), Title = "Book 1" },
            new() { LibraryMaterialId = Guid.NewGuid(), Title = "Book 2" }
        };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<Query.Execute>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetBooks();

        // Assert
        Assert.Equal(2, result.Value!.Count);
        Assert.Equal("Book 1", result.Value[0].Title);
    }

    [Fact]
    public async Task GetBooks_EmptyList_ReturnsEmpty()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<Query.Execute>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BookMaterialDto>());

        // Act
        var result = await _controller.GetBooks();

        // Assert
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task GetSingleBook_ReturnsBookFromMediator()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var expected = new BookMaterialDto
        {
            LibraryMaterialId = bookId,
            Title = "Single Book"
        };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<FilterQuery.SingleBook>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetSingleBook(bookId);

        // Assert
        Assert.Equal(bookId, result.Value!.LibraryMaterialId);
        Assert.Equal("Single Book", result.Value.Title);
    }

    [Fact]
    public async Task GetSingleBook_MediatorThrows_PropagatesException()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<FilterQuery.SingleBook>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Book not found"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.GetSingleBook(Guid.NewGuid()));
    }
}
