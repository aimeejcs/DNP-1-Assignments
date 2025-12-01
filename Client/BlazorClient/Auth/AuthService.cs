using ApiContracts;
using System.Net.Http.Json;

namespace BlazorClient.Auth;

public class AuthService : IAuthService
{
    private readonly HttpClient client;
    private UserDto? currentUser;

    public AuthService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto?> LoginAsync(string username, string password)
    {
        var request = new LoginRequest(username, password);
        var response = await client.PostAsJsonAsync("Auth/login", request);

        if (!response.IsSuccessStatusCode)
            return null;

        currentUser = await response.Content.ReadFromJsonAsync<UserDto>();
        return currentUser;
    }

    public Task LogoutAsync()
    {
        currentUser = null;
        return Task.CompletedTask;
    }

    public Task<UserDto?> GetCurrentUserAsync()
    {
        return Task.FromResult(currentUser);
    }
}
