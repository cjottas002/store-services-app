namespace StoreServices.Api.Author.Model;

public class AcademicDegree
{
    public int AcademicDegreeId { get; set; }
    public string? Name { get; set; }
    public string? AcademicCenter { get; set; }
    public DateTime? GraduationDate { get; set; }
    public int AuthorBookId { get; set; }
    public AuthorBook? AuthorBook { get; set; }
    public string? AcademicDegreeGuid { get; set; }
}
