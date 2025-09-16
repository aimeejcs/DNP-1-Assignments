using System.Threading.Tasks;
using Entities;
using RepositoryContracts;
using InMemoryRepositories;
using CLI.UI;


namespace CLI; 

public class Program
{
    public static async Task Main(string[] args)
    {
        
        IUserRepository userRepo    = new InMemoryUserRepository();
        IPostRepository postRepo    = new InMemoryPostRepository();
        ICommentRepository commentRepo = new InMemoryCommentRepository();

        
        var u1 = await userRepo.AddAsync(new User { UserName = "john", Password = "1234" });
        var u2 = await userRepo.AddAsync(new User { UserName = "jane", Password = "pass" });

        var p1 = await postRepo.AddAsync(new Post
        {
            Title = "Hello",
            Body = "This is my first post",
            UserId = u1.Id
        });

        _ = await postRepo.AddAsync(new Post
        {
            Title = "Another",
            Body = "Just checking",
            UserId = u2.Id
        });

        _ = await commentRepo.AddAsync(new Comment
        {
            Body = "Nice post!",
            UserId = u2.Id,
            PostId = p1.Id
        });


        var app = new CliApp(userRepo, postRepo, commentRepo);
        await app.StartAsync();
    }
}
