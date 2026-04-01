namespace StoreServices.Api.Author.Model;

public class AuthorBook
{
    public int AuthorBookId { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public ICollection<AcademicDegree>? AcademicDegrees { get; set; }
    public string? AuthorBookGuid { get; set; }
}
