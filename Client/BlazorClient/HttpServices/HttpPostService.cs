using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorClient.HttpServices;

public class HttpPostService : IPostService
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpPostService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<PostDto> CreatePostAsync(CreatePostDto request)
    {
        // FIX: Absolute path starts with "/"
        var httpResponse = await client.PostAsJsonAsync("/Posts", request);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<PostDto>(response, jsonOptions)!;
    }

    public async Task<IEnumerable<PostDto>> GetPostsAsync(string? titleContains = null, int? authorId = null, string? authorName = null)
    {
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(titleContains))
            query.Add($"titleContains={Uri.EscapeDataString(titleContains)}");

        if (authorId.HasValue)
            query.Add($"authorId={authorId.Value}");

        if (!string.IsNullOrWhiteSpace(authorName))
            query.Add($"authorName={Uri.EscapeDataString(authorName)}");

        string url = "/Posts"; // FIXED

        if (query.Count > 0)
            url += "?" + string.Join("&", query);

        var httpResponse = await client.GetAsync(url);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<IEnumerable<PostDto>>(response, jsonOptions)!;
    }

    public async Task<PostDto?> GetPostByIdAsync(int id)
    {
        var httpResponse = await client.GetAsync($"/Posts/{id}"); // FIXED
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            return null;

        return JsonSerializer.Deserialize<PostDto>(response, jsonOptions);
    }

    public async Task DeletePostAsync(int id)
    {
        var httpResponse = await client.DeleteAsync($"/Posts/{id}"); // FIXED
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);
    }
}
