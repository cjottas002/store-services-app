using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreServices.Api.Book.Model;
using StoreServices.Api.Book.Persistence;

namespace StoreServices.Api.Book.Application;

public class FilterQuery
{
    public class SingleBook : IRequest<BookMaterialDto>
    {
        public Guid? BookId { get; set; }
    }

    public class Handler(LibraryContext context, IMapper mapper) : IRequestHandler<SingleBook, BookMaterialDto>
    {
        public async Task<BookMaterialDto> Handle(SingleBook request, CancellationToken cancellationToken)
        {
            var book = await context.LibraryMaterial
                .Where(x => x.LibraryMaterialId == request.BookId)
                .FirstOrDefaultAsync(cancellationToken);

            if (book == null)
            {
                throw new Exception("Book not found");
            }

            var bookDto = mapper.Map<LibraryMaterial, BookMaterialDto>(book);
            return bookDto;
        }
    }
}
