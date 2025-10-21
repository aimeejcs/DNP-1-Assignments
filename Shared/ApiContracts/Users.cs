namespace ApiContracts;

public record UserDto(int Id, string UserName);
public class CreateUserDto
{
    public required string UserName { get; init; }
    public required string Password { get; init; } 
}
