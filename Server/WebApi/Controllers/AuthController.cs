using ApiContracts;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    // ⛔ Simple in-memory "session" (just for assignment)
    private static readonly Dictionary<string, string> loggedInUsers = new();

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Very simple authentication: username == password
        if (request.Username != request.Password)
            return Unauthorized("Invalid credentials");

        // Create a fake session
        string sessionId = Guid.NewGuid().ToString();
        loggedInUsers[sessionId] = request.Username;

        return Ok(new LoginResponse
{
    SessionId = sessionId,
    Username = request.Username
});

    }

    [HttpPost("logout")]
    public IActionResult Logout([FromBody] string sessionId)
    {
        loggedInUsers.Remove(sessionId);
        return Ok();
    }

    [HttpGet("current")]
    public IActionResult Current([FromQuery] string sessionId)
    {
        if (sessionId != null && loggedInUsers.ContainsKey(sessionId))
            return Ok(loggedInUsers[sessionId]);

        return Ok(null);
    }
}
