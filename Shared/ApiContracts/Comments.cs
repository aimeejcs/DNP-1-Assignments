namespace ApiContracts;

public record CommentDto(int Id, int PostId, int AuthorId, string AuthorName, string Body);
public class CreateCommentDto
{
    public required int PostId { get; init; }
    public required int AuthorId { get; init; }
    public required string Body { get; init; }
}
