using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // POST /Users
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto request)
    {
        // Check if username is already taken
        var existing = _userRepository.GetManyAsync()
            .FirstOrDefault(u => u.UserName == request.UserName);
        if (existing is not null)
        {
            return Conflict($"Username '{request.UserName}' is already taken.");
        }

        var user = new User
        {
            UserName = request.UserName,
            Password = request.Password
        };

        var created = await _userRepository.AddAsync(user);
        var dto = MapToDto(created);

        return Created($"/Users/{dto.Id}", dto);
    }

    // GET /Users?userNameContains=han
    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetUsers([FromQuery] string? userNameContains)
    {
        var query = _userRepository.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(userNameContains))
        {
            query = query.Where(u =>
                u.UserName.Contains(userNameContains, StringComparison.OrdinalIgnoreCase));
        }

        var result = query
            .Select(MapToDto)
            .ToList();

        return Ok(result);
    }

    // GET /Users/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        try
        {
            var user = await _userRepository.GetSingleAsync(id);
            return Ok(MapToDto(user));
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with id {id} was not found.");
        }
    }

    // PUT /Users/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUserDto request)
    {
        try
        {
            var user = await _userRepository.GetSingleAsync(id);

            user.UserName = request.UserName;
            user.Password = request.Password;

            await _userRepository.UpdateAsync(user);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with id {id} was not found.");
        }
    }

    // DELETE /Users/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound($"User with id {id} was not found.");
        }
    }

    private static UserDto MapToDto(User user)
        => new(user.Id, user.UserName);
}
