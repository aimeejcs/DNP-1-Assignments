using ApiContracts;

namespace BlazorClient.Auth;

public class SimpleAuthProvider
{
    private readonly HttpClient _client;

    public bool IsLoggedIn => CurrentUser != null;
    public LoginResponse? CurrentUser { get; private set; }

    public SimpleAuthProvider(HttpClient client)
    {
        _client = client;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var request = new LoginRequest
        {
            Username = username,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("auth/login", request);

        if (!response.IsSuccessStatusCode)
            return false;

        CurrentUser = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return true;
    }

    public async Task LogoutAsync()
    {
        await _client.PostAsync("auth/logout", null);
        CurrentUser = null;
    }

    public async Task<bool> CheckSessionAsync()
    {
        var response = await _client.GetAsync("auth/me");

        if (!response.IsSuccessStatusCode)
        {
            CurrentUser = null;
            return false;
        }

        CurrentUser = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return true;
    }
}
