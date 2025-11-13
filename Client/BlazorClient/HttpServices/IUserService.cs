using ApiContracts;

namespace BlazorClient.HttpServices;

public interface IUserService
{
    Task<UserDto> AddUserAsync(CreateUserDto request);
    Task<IEnumerable<UserDto>> GetUsersAsync(string? userNameContains = null);
    Task<UserDto?> GetUserByIdAsync(int id);
}
