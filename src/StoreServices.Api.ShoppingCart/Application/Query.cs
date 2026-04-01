using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreServices.Api.ShoppingCart.Persistence;
using StoreServices.Api.ShoppingCart.RemoteInterface;

namespace StoreServices.Api.ShoppingCart.Application;

public class Query
{
    public class Execute : IRequest<CartDto>
    {
        public int CartSessionId { get; set; }
    }

    public class Handler(CartContext context, IBooksService bookService) : IRequestHandler<Execute, CartDto>
    {
        public async Task<CartDto> Handle(Execute request, CancellationToken cancellationToken)
        {
            var cartSession = await context.CartSession
                .FirstOrDefaultAsync(x => x.CartSessionId == request.CartSessionId, cancellationToken);

            var cartSessionDetails = await context.CartSessionDetail
                .Where(x => x.CartSessionId == request.CartSessionId)
                .ToListAsync(cancellationToken);

            var cartDetailDtos = new List<CartDetailDto>();

            foreach (var item in cartSessionDetails)
            {
                var response = await bookService.GetBook(new Guid(item.SelectedProduct!));
                if (response.result)
                {
                    var bookObj = response.Book!;
                    var cartDetail = new CartDetailDto
                    {
                        BookTitle = bookObj.Title,
                        PublicationDate = bookObj.PublicationDate,
                        BookId = bookObj.LibraryMaterialId
                    };
                    cartDetailDtos.Add(cartDetail);
                }
            }

            var cartSessionDto = new CartDto
            {
                CartId = cartSession!.CartSessionId,
                SessionCreationDate = cartSession.CreationDate,
                Products = cartDetailDtos
            };

            return cartSessionDto;
        }
    }
}
