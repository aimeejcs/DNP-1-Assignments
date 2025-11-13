using ApiContracts;

namespace BlazorClient.HttpServices;

public interface ICommentService
{
    Task<CommentDto> CreateCommentAsync(CreateCommentDto request);
    Task<IEnumerable<CommentDto>> GetCommentsForPostAsync(int postId);
}
