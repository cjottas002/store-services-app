using FluentValidation;
using MediatR;
using StoreServices.Api.Author.Model;
using StoreServices.Api.Author.Persistence;

namespace StoreServices.Api.Author.Application;

public class Create
{
    public class Execute : IRequest
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
    }

    public class ExecuteValidation : AbstractValidator<Execute>
    {
        public ExecuteValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
    }

    public class Handler(AuthorContext context) : IRequestHandler<Execute>
    {
        public async Task Handle(Execute request, CancellationToken cancellationToken)
        {
            var authorBook = new AuthorBook
            {
                Name = request.Name,
                BirthDate = request.BirthDate,
                LastName = request.LastName,
                AuthorBookGuid = Convert.ToString(Guid.NewGuid())
            };

            context.AuthorBook.Add(authorBook);
            var value = await context.SaveChangesAsync(cancellationToken);

            if (value <= 0)
            {
                throw new Exception("Could not insert the book author");
            }
        }
    }
}
