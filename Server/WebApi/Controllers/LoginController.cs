using ApiContracts;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private static readonly List<LoginRequest> validUsers = new()
    {
        new LoginRequest("YxngHamzyz", "1234"),
        new LoginRequest("Hamuushh", "abcd"),
    };

    [HttpPost]
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        var match = validUsers.FirstOrDefault(u =>
            u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == request.Password);

        if (match is null)
        {
            return Unauthorized("Invalid username or password");
        }

        // Return a fake authenticated user
        return Ok(new UserDto(1, request.Username));

    }
}
