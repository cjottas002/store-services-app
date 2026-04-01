namespace StoreServices.Api.Book.Application;

public record BookMaterialDto
{
    public Guid? LibraryMaterialId { get; set; }
    public string? Title { get; set; }
    public DateTime? PublicationDate { get; set; }
    public Guid? AuthorBook { get; set; }
}
