using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository _userRepo;
    private readonly IPostRepository _postRepo;
    private readonly ICommentRepository _commentRepo;

    public CliApp(IUserRepository userRepo, IPostRepository postRepo, ICommentRepository commentRepo)
    {
        _userRepo = userRepo;
        _postRepo = postRepo;
        _commentRepo = commentRepo;
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine("\n=== Main Menu ===");
            Console.WriteLine("1) Manage Users");
            Console.WriteLine("2) Manage Posts");
            Console.WriteLine("0) Exit");
            Console.Write("Choose: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await new ManageUsersView(_userRepo).ShowAsync();
                    break;
                case "2":
                    await new ManagePostsView(_postRepo, _commentRepo).ShowAsync();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
