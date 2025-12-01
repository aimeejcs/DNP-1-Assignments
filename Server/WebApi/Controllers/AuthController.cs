using ApiContracts;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        // TEMPORARY MOCK LOGIN
        if (request.Username == "user" && request.Password == "123")
        {
            return Ok(new UserDto(1, "user"));
        }

        return Unauthorized("Invalid credentials");
    }
}
