using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;

    public CommentsController(
        ICommentRepository commentRepository,
        IUserRepository userRepository,
        IPostRepository postRepository)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
        _postRepository = postRepository;
    }

    // POST /Comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto request)
    {
        // Validate post exists
        try
        {
            await _postRepository.GetSingleAsync(request.PostId);
        }
        catch (InvalidOperationException)
        {
            return BadRequest($"Post with id {request.PostId} does not exist.");
        }

        // Validate author exists
        try
        {
            await _userRepository.GetSingleAsync(request.AuthorId);
        }
        catch (InvalidOperationException)
        {
            return BadRequest($"User with id {request.AuthorId} does not exist.");
        }

        var comment = new Comment
        {
            PostId = request.PostId,
            UserId = request.AuthorId,
            Body = request.Body
        };

        var created = await _commentRepository.AddAsync(comment);
        var dto = await MapToDtoAsync(created);

        return Created($"/Comments/{dto.Id}", dto);
    }

    // GET /Comments?userId=1&userName=han&postId=2
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? postId)
    {
        var query = _commentRepository.GetManyAsync();

        if (userId is not null)
        {
            query = query.Where(c => c.UserId == userId.Value);
        }

        if (postId is not null)
        {
            query = query.Where(c => c.PostId == postId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            var matchingUserIds = _userRepository.GetManyAsync()
                .Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();

            if (matchingUserIds.Count > 0)
            {
                query = query.Where(c => matchingUserIds.Contains(c.UserId));
            }
            else
            {
                return Ok(Array.Empty<CommentDto>());
            }
        }

        var comments = query.ToList();

        var dtos = new List<CommentDto>();
        foreach (var comment in comments)
        {
            dtos.Add(await MapToDtoAsync(comment));
        }

        return Ok(dtos);
    }

    // GET /Comments/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetCommentById(int id)
    {
        try
        {
            var comment = await _commentRepository.GetSingleAsync(id);
            var dto = await MapToDtoAsync(comment);
            return Ok(dto);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with id {id} was not found.");
        }
    }

    // PUT /Comments/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] CreateCommentDto request)
    {
        try
        {
            var comment = await _commentRepository.GetSingleAsync(id);

            comment.Body = request.Body;
            comment.PostId = request.PostId;
            comment.UserId = request.AuthorId;

            await _commentRepository.UpdateAsync(comment);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with id {id} was not found.");
        }
    }

    // DELETE /Comments/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        try
        {
            await _commentRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with id {id} was not found.");
        }
    }

    private async Task<CommentDto> MapToDtoAsync(Comment comment)
    {
        string authorName;
        try
        {
            var author = await _userRepository.GetSingleAsync(comment.UserId);
            authorName = author.UserName;
        }
        catch (InvalidOperationException)
        {
            authorName = string.Empty;
        }

        return new CommentDto(
            comment.Id,
            comment.PostId,
            comment.UserId,
            authorName,
            comment.Body);
    }
}
