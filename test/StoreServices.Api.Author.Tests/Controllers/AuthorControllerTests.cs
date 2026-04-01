using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.Author.Application;
using StoreServices.Api.Author.Controllers;
using Xunit;

namespace StoreServices.Api.Author.Tests.Controllers;

public class AuthorControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AuthorController _controller;

    public AuthorControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AuthorController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Create_ReturnsOkResult()
    {
        // Arrange
        var request = new Create.Execute
        {
            Name = "Robert",
            LastName = "Martin",
            BirthDate = new DateTime(1952, 12, 5)
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
        var request = new Create.Execute { Name = "Martin", LastName = "Fowler" };
        _mediatorMock
            .Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.Create(request);

        // Assert
        _mediatorMock.Verify(m => m.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAuthors_ReturnsListFromMediator()
    {
        // Arrange
        var expected = new List<AuthorDto>
        {
            new() { Name = "Robert", LastName = "Martin", AuthorBookGuid = Guid.NewGuid().ToString() },
            new() { Name = "Martin", LastName = "Fowler", AuthorBookGuid = Guid.NewGuid().ToString() }
        };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<Query.AuthorList>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetAuthors();

        // Assert
        Assert.Equal(2, result.Value!.Count);
        Assert.Equal("Robert", result.Value[0].Name);
    }

    [Fact]
    public async Task GetAuthors_EmptyList_ReturnsEmpty()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<Query.AuthorList>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AuthorDto>());

        // Act
        var result = await _controller.GetAuthors();

        // Assert
        Assert.Empty(result.Value!);
    }

    [Fact]
    public async Task GetAuthorBook_ReturnsAuthorFromMediator()
    {
        // Arrange
        var guid = Guid.NewGuid().ToString();
        var expected = new AuthorDto
        {
            Name = "Eric",
            LastName = "Evans",
            AuthorBookGuid = guid
        };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<FilterQuery.SingleAuthor>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetAuthorBook(guid);

        // Assert
        Assert.Equal(guid, result.Value!.AuthorBookGuid);
        Assert.Equal("Eric", result.Value.Name);
    }

    [Fact]
    public async Task GetAuthorBook_MediatorThrows_PropagatesException()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<FilterQuery.SingleAuthor>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Author not found"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.GetAuthorBook("non-existent"));
    }
}
