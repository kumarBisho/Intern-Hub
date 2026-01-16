using InternMS.Api.Services;
using InternMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using InternMS.Domain.Entities;
using InternMS.Api.DTOs.Users;
using InternMS.Api.DTOs.Profiles;
using AutoMapper;

namespace InternMS.Api.Services.Users
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public UserService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _db.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Profile)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _db.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateUserAsync(Guid id, UserDto updatedUser)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if( user == null)
            {
                throw new Exception("User not found");
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.IsActive = updatedUser.IsActive;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task<UserProfile?> GetProfileAsync(Guid userId)
        {
            return await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
        {
            var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);

            if( profile == null )
            {
                profile = new UserProfile { UserId = userId };
                _db.Profiles.Add(profile);
            }

            _mapper.Map(dto, profile);
            await _db.SaveChangesAsync();
        }
    }
}