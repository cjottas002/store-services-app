using StoreServices.Api.ShoppingCart.RemoteModel;

namespace StoreServices.Api.ShoppingCart.RemoteInterface;

public interface IBooksService
{
    Task<(bool result, BookRemote? Book, string? ErrorMessage)> GetBook(Guid BookId);
}
