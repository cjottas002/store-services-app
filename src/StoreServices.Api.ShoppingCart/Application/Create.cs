using MediatR;
using StoreServices.Api.ShoppingCart.Model;
using StoreServices.Api.ShoppingCart.Persistence;

namespace StoreServices.Api.ShoppingCart.Application;

public class Create
{
    public class Execute : IRequest
    {
        public DateTime SessionCreationDate { get; set; }
        public List<string>? ProductList { get; set; }
    }

    public class Handler(CartContext context) : IRequestHandler<Execute>
    {
        public async Task Handle(Execute request, CancellationToken cancellationToken)
        {
            var cartSession = new CartSession
            {
                CreationDate = request.SessionCreationDate
            };

            context.CartSession.Add(cartSession);
            var value = await context.SaveChangesAsync(cancellationToken);

            if (value == 0)
            {
                throw new Exception("Error inserting the shopping cart");
            }

            int id = cartSession.CartSessionId;

            foreach (var obj in request.ProductList!)
            {
                var sessionDetail = new CartSessionDetail
                {
                    CreationDate = DateTime.Now,
                    CartSessionId = id,
                    SelectedProduct = obj
                };

                context.CartSessionDetail.Add(sessionDetail);
            }

            value = await context.SaveChangesAsync(cancellationToken);

            if (value == 0)
            {
                throw new Exception("Could not insert the shopping cart detail");
            }
        }
    }
}
