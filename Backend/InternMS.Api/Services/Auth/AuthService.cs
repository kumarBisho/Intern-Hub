using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using InternMS.Infrastructure.Data;
using InternMS.Domain.Entities;
using System;

namespace InternMS.Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;

        public AuthService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _db.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if(user == null) return null;

            bool Verified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return Verified? user : null ;
        }

        public async Task<User?> RegisterUserAsync(string email, string password, string firstName, string lastName, int roleId)
        {
            if(await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            {
                throw new InvalidOperationException("Email already in use");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            };
            _db.UserRoles.Add(userRole);

            await _db.SaveChangesAsync();

            await _db.Entry(user).Collection(u => u.UserRoles).LoadAsync();

            return user;
        }

        public async Task LogoutAsync(string token)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var expiryDate = jwt.ValidTo;
            var blacklistedToken = new BlacklistedToken
            {
                Token = token,
                ExpiryDate = expiryDate,
                RevokedAt = DateTime.UtcNow
            };
            _db.BlacklistedTokens.Add(blacklistedToken);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _db.BlacklistedTokens.AnyAsync(t=> t.Token == token && t.ExpiryDate > DateTime.UtcNow);
        }

    }
}