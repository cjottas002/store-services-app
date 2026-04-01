namespace StoreServices.Api.ShoppingCart.Application;

public record CartDto
{
    public int CartId { get; set; }
    public DateTime? SessionCreationDate { get; set; }
    public List<CartDetailDto>? Products { get; set; }
}
