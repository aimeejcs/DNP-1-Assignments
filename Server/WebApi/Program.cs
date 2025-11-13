using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RepositoryContracts;
using InMemoryRepositories;

var builder = WebApplication.CreateBuilder(args);

// Add controller support
builder.Services.AddControllers();

// Add Swagger for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection — use your InMemory repositories
builder.Services.AddSingleton<IUserRepository, UserInMemoryRepository>();
builder.Services.AddSingleton<IPostRepository, PostInMemoryRepository>();
builder.Services.AddSingleton<ICommentRepository, CommentInMemoryRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
