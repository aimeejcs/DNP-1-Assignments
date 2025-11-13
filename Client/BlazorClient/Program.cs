using BlazorClient.Components;
using BlazorClient.HttpServices;
using BlazorClient.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Your API Base URL (Server/WebApi is running on http://localhost:5045)
string apiBaseUrl = "http://localhost:5045/";
Console.WriteLine("[DEBUG] API Base URL = " + apiBaseUrl);

// Configure HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// Register HTTP Services
builder.Services.AddScoped<IUserService, HttpUserService>();
builder.Services.AddScoped<IPostService, HttpPostService>();
builder.Services.AddScoped<ICommentService, HttpCommentService>();
builder.Services.AddScoped<SimpleAuthProvider>();


var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Setup Razor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
