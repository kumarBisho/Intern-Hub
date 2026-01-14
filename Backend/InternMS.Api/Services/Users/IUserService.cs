using InternMS.Api.DTOs.Users;
using InternMS.Api.DTOs.Profiles;
using InternMS.Domain.Entities;


namespace InternMS.Api.Services.Users   
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task UpdateUserAsync(Guid id, UserDto updatedUser);
        Task<UserProfile?> GetProfileAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
    }
}