using System.Net.Http.Json;
using System.Text.Json;
using ApiContracts;

namespace BlazorClient.HttpServices;

public class HttpCommentService : ICommentService
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpCommentService(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CommentDto> CreateCommentAsync(CreateCommentDto request)
    {
        // FIX: Absolute route must start with "/"
        var httpResponse = await client.PostAsJsonAsync("/Comments", request);
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<CommentDto>(response, jsonOptions)!;
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsForPostAsync(int postId)
    {
        // FIX: Also needs leading "/"
        var httpResponse = await client.GetAsync($"/Comments?postId={postId}");
        var response = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception(response);

        return JsonSerializer.Deserialize<IEnumerable<CommentDto>>(response, jsonOptions)!;
    }
}
