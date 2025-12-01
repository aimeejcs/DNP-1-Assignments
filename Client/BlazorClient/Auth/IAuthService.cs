using ApiContracts;

namespace BlazorClient.Auth;

public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
    Task<UserDto?> GetCurrentUserAsync();
}
