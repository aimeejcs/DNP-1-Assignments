using Entities;
using RepositoryContracts;
using InMemoryRepositories;      // <- IMPORTANT
using CLI;

var userRepo    = new InMemoryUserRepository();
var postRepo    = new InMemoryPostRepository();
var commentRepo = new InMemoryCommentRepository();

// ---- Seed using object initializers (no ctor args) ----
var u1 = await userRepo.AddAsync(new User { UserName = "john", Password = "1234" });
var u2 = await userRepo.AddAsync(new User { UserName = "jane", Password = "pass" });

var p1 = await postRepo.AddAsync(new Post { Title = "Hello",   Body = "This is my first post", UserId = u1.Id });
var p2 = await postRepo.AddAsync(new Post { Title = "Another", Body = "Just checking",         UserId = u2.Id });

await commentRepo.AddAsync(new Comment { Body = "Nice post!", UserId = u2.Id, PostId = p1.Id });
// -------------------------------------------------------

var app = new CliApp(userRepo, postRepo, commentRepo);
await app.StartAsync();

