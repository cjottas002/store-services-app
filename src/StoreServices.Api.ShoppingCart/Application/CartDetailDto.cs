namespace StoreServices.Api.ShoppingCart.Application;

public record CartDetailDto
{
    public Guid? BookId { get; set; }
    public string? BookTitle { get; set; }
    public string? AuthorBook { get; set; }
    public DateTime? PublicationDate { get; set; }
}
