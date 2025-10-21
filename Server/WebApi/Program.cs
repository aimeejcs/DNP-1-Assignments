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
// Add these when you show me their interfaces later:
//// builder.Services.AddSingleton<IPostRepository, InMemoryPostRepository>();
//// builder.Services.AddSingleton<ICommentRepository, InMemoryCommentRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
