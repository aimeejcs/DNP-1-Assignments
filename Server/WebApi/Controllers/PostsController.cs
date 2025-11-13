using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;

    public PostsController(IPostRepository postRepository, IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    // POST /Posts
    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto request)
    {
        // Validate that author exists
        try
        {
            await _userRepository.GetSingleAsync(request.AuthorId);
        }
        catch (InvalidOperationException)
        {
            return BadRequest($"User with id {request.AuthorId} does not exist.");
        }

        var post = new Post
        {
            Title = request.Title,
            Body = request.Body,
            UserId = request.AuthorId
        };

        var created = await _postRepository.AddAsync(post);
        var dto = await MapToDtoAsync(created);

        return Created($"/Posts/{dto.Id}", dto);
    }

    // GET /Posts?titleContains=Hello&authorId=1&authorName=ham
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts(
        [FromQuery] string? titleContains,
        [FromQuery] int? authorId,
        [FromQuery] string? authorName)
    {
        var query = _postRepository.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            query = query.Where(p =>
                p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));
        }

        if (authorId is not null)
        {
            query = query.Where(p => p.UserId == authorId.Value);
        }

        if (!string.IsNullOrWhiteSpace(authorName))
        {
            var matchingUserIds = _userRepository.GetManyAsync()
                .Where(u => u.UserName.Contains(authorName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();

            if (matchingUserIds.Count > 0)
            {
                query = query.Where(p => matchingUserIds.Contains(p.UserId));
            }
            else
            {
                return Ok(Array.Empty<PostDto>());
            }
        }

        var posts = query.ToList();

        var dtos = new List<PostDto>();
        foreach (var post in posts)
        {
            dtos.Add(await MapToDtoAsync(post));
        }

        return Ok(dtos);
    }

    // GET /Posts/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPostById(int id)
    {
        try
        {
            var post = await _postRepository.GetSingleAsync(id);
            var dto = await MapToDtoAsync(post);
            return Ok(dto);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post with id {id} was not found.");
        }
    }

    // PUT /Posts/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] CreatePostDto request)
    {
        try
        {
            var post = await _postRepository.GetSingleAsync(id);

            post.Title = request.Title;
            post.Body = request.Body;
            post.UserId = request.AuthorId;

            await _postRepository.UpdateAsync(post);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post with id {id} was not found.");
        }
    }

    // DELETE /Posts/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            await _postRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post with id {id} was not found.");
        }
    }

    private async Task<PostDto> MapToDtoAsync(Post post)
    {
        string authorName;
        try
        {
            var author = await _userRepository.GetSingleAsync(post.UserId);
            authorName = author.UserName;
        }
        catch (InvalidOperationException)
        {
            authorName = string.Empty;
        }

        return new PostDto(
            post.Id,
            post.Title,
            post.Body,
            post.UserId,
            authorName);
    }
}
