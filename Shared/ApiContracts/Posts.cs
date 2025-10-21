namespace ApiContracts;

public record PostDto(int Id, string Title, string Body, int AuthorId, string AuthorName);
public class CreatePostDto
{
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required int AuthorId { get; init; }
}
