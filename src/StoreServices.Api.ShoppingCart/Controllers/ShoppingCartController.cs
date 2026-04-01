using MediatR;
using Microsoft.AspNetCore.Mvc;
using StoreServices.Api.ShoppingCart.Application;

namespace StoreServices.Api.ShoppingCart.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShoppingCartController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(Create.Execute data)
    {
        await mediator.Send(data);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CartDto>> GetCart(int id)
    {
        return await mediator.Send(new Query.Execute { CartSessionId = id });
    }
}
