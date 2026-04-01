using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreServices.Api.Author.Model;
using StoreServices.Api.Author.Persistence;

namespace StoreServices.Api.Author.Application;

public class FilterQuery
{
    public class SingleAuthor : IRequest<AuthorDto>
    {
        public string? AuthorGuid { get; set; }
    }

    public class Handler(AuthorContext context, IMapper mapper) : IRequestHandler<SingleAuthor, AuthorDto>
    {
        public async Task<AuthorDto> Handle(SingleAuthor request, CancellationToken cancellationToken)
        {
            var author = await context.AuthorBook
                .Where(x => x.AuthorBookGuid == request.AuthorGuid)
                .FirstOrDefaultAsync(cancellationToken);

            if (author == null)
            {
                throw new Exception("Author not found");
            }

            var authorDto = mapper.Map<AuthorBook, AuthorDto>(author);
            return authorDto;
        }
    }
}
