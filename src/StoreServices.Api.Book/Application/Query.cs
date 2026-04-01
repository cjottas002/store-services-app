using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreServices.Api.Book.Model;
using StoreServices.Api.Book.Persistence;

namespace StoreServices.Api.Book.Application;

public class Query
{
    public class Execute : IRequest<List<BookMaterialDto>> { }

    public class Handler(LibraryContext context, IMapper mapper) : IRequestHandler<Execute, List<BookMaterialDto>>
    {
        public async Task<List<BookMaterialDto>> Handle(Execute request, CancellationToken cancellationToken)
        {
            var books = await context.LibraryMaterial.ToListAsync(cancellationToken);
            var booksDto = mapper.Map<List<LibraryMaterial>, List<BookMaterialDto>>(books);
            return booksDto;
        }
    }
}
