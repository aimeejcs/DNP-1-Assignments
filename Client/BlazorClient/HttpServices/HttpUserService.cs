using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorClient.HttpServices;

public class HttpUserService : IUserService
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpUserService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<UserDto> AddUserAsync(CreateUserDto request)
    {
        // IMPORTANT: absolute path starts with "/"
        var httpResponse = await client.PostAsJsonAsync("/Users", request);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<UserDto>(response, jsonOptions)!;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync(string? userNameContains = null)
    {
        string url = "/Users"; // FIXED

        if (!string.IsNullOrWhiteSpace(userNameContains))
            url += $"?userNameContains={Uri.EscapeDataString(userNameContains)}";

        var httpResponse = await client.GetAsync(url);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<IEnumerable<UserDto>>(response, jsonOptions)!;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var httpResponse = await client.GetAsync($"/Users/{id}"); // FIXED
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            return null;

        return JsonSerializer.Deserialize<UserDto>(response, jsonOptions);
    }
}
