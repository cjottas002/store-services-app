using MediatR;
using Microsoft.AspNetCore.Mvc;
using StoreServices.Api.Author.Application;

namespace StoreServices.Api.Author.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(Create.Execute data)
    {
        await mediator.Send(data);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<List<AuthorDto>>> GetAuthors()
    {
        return await mediator.Send(new Query.AuthorList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetAuthorBook(string id)
    {
        return await mediator.Send(new FilterQuery.SingleAuthor { AuthorGuid = id });
    }
}
