using ApiContracts;

namespace BlazorClient.HttpServices;

public interface IPostService
{
    Task<PostDto> CreatePostAsync(CreatePostDto request);
    Task<IEnumerable<PostDto>> GetPostsAsync(string? titleContains = null, int? authorId = null, string? authorName = null);
    Task<PostDto?> GetPostByIdAsync(int id);
    Task DeletePostAsync(int id);
}
