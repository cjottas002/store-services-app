using FluentValidation;
using MediatR;
using StoreServices.Api.Book.Model;
using StoreServices.Api.Book.Persistence;

namespace StoreServices.Api.Book.Application;

public class Create
{
    public class Execute : IRequest
    {
        public string? Title { get; set; }
        public DateTime? PublicationDate { get; set; }
        public Guid? AuthorBook { get; set; }
    }

    public class ExecuteValidation : AbstractValidator<Execute>
    {
        public ExecuteValidation()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.PublicationDate).NotEmpty();
            RuleFor(x => x.AuthorBook).NotEmpty();
        }
    }

    public class Handler(LibraryContext context) : IRequestHandler<Execute>
    {
        public async Task Handle(Execute request, CancellationToken cancellationToken)
        {
            var book = new LibraryMaterial
            {
                Title = request.Title,
                PublicationDate = request.PublicationDate,
                AuthorBook = request.AuthorBook
            };

            context.LibraryMaterial.Add(book);
            var value = await context.SaveChangesAsync(cancellationToken);

            if (value == 0)
            {
                throw new Exception("Could not save the book");
            }
        }
    }
}
