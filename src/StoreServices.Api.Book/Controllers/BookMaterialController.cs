using MediatR;
using Microsoft.AspNetCore.Mvc;
using StoreServices.Api.Book.Application;

namespace StoreServices.Api.Book.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookMaterialController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(Create.Execute data)
    {
        await mediator.Send(data);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<List<BookMaterialDto>>> GetBooks()
    {
        return await mediator.Send(new Query.Execute());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookMaterialDto>> GetSingleBook(Guid id)
    {
        return await mediator.Send(new FilterQuery.SingleBook { BookId = id });
    }
}
