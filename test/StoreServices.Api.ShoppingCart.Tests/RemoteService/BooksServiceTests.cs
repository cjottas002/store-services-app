using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using StoreServices.Api.ShoppingCart.RemoteModel;
using StoreServices.Api.ShoppingCart.RemoteService;
using Xunit;

namespace StoreServices.Api.ShoppingCart.Tests.RemoteService;

public class BooksServiceTests
{
    private BooksService CreateService(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock
            .Setup(f => f.CreateClient("Books"))
            .Returns(httpClient);

        var loggerMock = new Mock<ILogger<BooksService>>();

        return new BooksService(factoryMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task GetBook_Success_ReturnsBookData()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new BookRemote
        {
            LibraryMaterialId = bookId,
            Title = "Clean Code",
            PublicationDate = new DateTime(2008, 8, 1),
            AuthorBook = Guid.NewGuid()
        };
        var json = JsonSerializer.Serialize(book);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        var service = CreateService(response);

        // Act
        var result = await service.GetBook(bookId);

        // Assert
        Assert.True(result.result);
        Assert.NotNull(result.Book);
        Assert.Equal("Clean Code", result.Book!.Title);
        Assert.Equal(bookId, result.Book.LibraryMaterialId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task GetBook_NotFound_ReturnsFalseWithReason()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            ReasonPhrase = "Not Found"
        };
        var service = CreateService(response);

        // Act
        var result = await service.GetBook(Guid.NewGuid());

        // Assert
        Assert.False(result.result);
        Assert.Null(result.Book);
        Assert.Equal("Not Found", result.ErrorMessage);
    }

    [Fact]
    public async Task GetBook_ServerError_ReturnsFalse()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            ReasonPhrase = "Internal Server Error"
        };
        var service = CreateService(response);

        // Act
        var result = await service.GetBook(Guid.NewGuid());

        // Assert
        Assert.False(result.result);
        Assert.Null(result.Book);
        Assert.Equal("Internal Server Error", result.ErrorMessage);
    }

    [Fact]
    public async Task GetBook_HttpException_ReturnsFalseWithMessage()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));
        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient("Books")).Returns(httpClient);
        var loggerMock = new Mock<ILogger<BooksService>>();
        var service = new BooksService(factoryMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetBook(Guid.NewGuid());

        // Assert
        Assert.False(result.result);
        Assert.Null(result.Book);
        Assert.Contains("Connection refused", result.ErrorMessage);
    }

    [Fact]
    public async Task GetBook_Success_DeserializesCaseInsensitive()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var json = $"{{\"libraryMaterialId\":\"{bookId}\",\"title\":\"PascalCase Test\",\"publicationDate\":\"2023-01-01T00:00:00\",\"authorBook\":\"{Guid.NewGuid()}\"}}";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        var service = CreateService(response);

        // Act
        var result = await service.GetBook(bookId);

        // Assert
        Assert.True(result.result);
        Assert.Equal("PascalCase Test", result.Book!.Title);
    }
}
