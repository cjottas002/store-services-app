using System.Text.Json;
using StoreServices.Api.ShoppingCart.RemoteInterface;
using StoreServices.Api.ShoppingCart.RemoteModel;

namespace StoreServices.Api.ShoppingCart.RemoteService;

public class BooksService(IHttpClientFactory httpClient, ILogger<BooksService> logger) : IBooksService
{
    public async Task<(bool result, BookRemote? Book, string? ErrorMessage)> GetBook(Guid BookId)
    {
        try
        {
            var client = httpClient.CreateClient("Books");
            var response = await client.GetAsync($"api/BookMaterial/{BookId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<BookRemote>(content, options);
                return (true, result, null);
            }

            return (false, null, response.ReasonPhrase);
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
            return (false, null, e.Message);
        }
    }
}
