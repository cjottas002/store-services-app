using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreServices.Api.Author.Model;
using StoreServices.Api.Author.Persistence;

namespace StoreServices.Api.Author.Application;

public class Query
{
    public class AuthorList : IRequest<List<AuthorDto>> { }

    public class Handler(AuthorContext context, IMapper mapper) : IRequestHandler<AuthorList, List<AuthorDto>>
    {
        public async Task<List<AuthorDto>> Handle(AuthorList request, CancellationToken cancellationToken)
        {
            var authors = await context.AuthorBook.ToListAsync(cancellationToken);
            var authorsDto = mapper.Map<List<AuthorBook>, List<AuthorDto>>(authors);
            return authorsDto;
        }
    }
}
