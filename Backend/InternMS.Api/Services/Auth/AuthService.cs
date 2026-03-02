using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using InternMS.Infrastructure.Data;
using InternMS.Domain.Entities;
using InternMS.Api.Services.Email;
using InternMS.Api.Services.Token;
using InternMS.Api.DTOs.Authentication; 
using System;
using Microsoft.AspNetCore.Http.HttpResults;


namespace InternMS.Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public AuthService(AppDbContext db, IEmailService emailService, ITokenService tokenService)
        {
            _db = db;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        public async Task<User?> ConfirmEmailAsync(string token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u=>u.EmailConfirmationToken == token);
            if(user == null || user.EmailConfirmationTokenExpires < DateTime.UtcNow)
            {
                return null;
            }
            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.EmailConfirmationTokenExpires = null;
            if (user.AdminApproved)
            {
                user.IsActive = true;
            }
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ApproveUserAsync(Guid userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u=>u.Id == userId);
            if(user == null)
            {
                return false;
            }
            user.AdminApproved = true;
            if (user.EmailConfirmed)
            {
                user.IsActive = true;
            }
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<User?> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _db.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if(user == null) return null;

            if(!user.EmailConfirmed || !user.AdminApproved)
            {
                throw new Exception("Account not verified.");
            }

            bool Verified = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            return Verified? user : null ;
        }

        public async Task<User?> RegisterUserAsync(string email, string password, string firstName, string lastName, int roleId)
        {
            if(await _db.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            {
                throw new InvalidOperationException("Email already in use");
            }

            var token = Guid.NewGuid().ToString();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmationToken = token,
                EmailConfirmationTokenExpires = DateTime.UtcNow.AddHours(24),
                IsActive = false,
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

            var confirmLink = $"http://localhost:5248/api/auth/confirm-email?token={token}";
            var verifyUserLink = $"http://localhost:5248/api/auth/approve-user?userId={user.Id}";
            
            await _emailService.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your email by clicking <a href='{confirmLink}'>here</a>. This link will expire in 24 hours."
            );

            await _emailService.SendEmailAsync(
                "2021uec1535@mnit.ac.in",
                "New User Registration",
                $"A new user has registered with the email: {email}. Please review and approve the account by clicking <a href='{verifyUserLink}'>here</a>."
            );

            await _db.Entry(user).Collection(u => u.UserRoles).LoadAsync();

            return user;
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _db.RefreshTokens.FirstOrDefaultAsync(t=>t.Token == refreshToken);

            if(token != null)
            {
                token.IsRevoked = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _db.BlacklistedTokens.AnyAsync(t=> t.Token == token && t.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _db.RefreshTokens.Include(r=>r.User).ThenInclude(u=>u.UserRoles).ThenInclude(ur=>ur.Role).FirstOrDefaultAsync(rt=>rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);

            if(storedToken == null) return null;

            var roles = storedToken.User.UserRoles?.Select(r=>r.Role.Name)?? new string[0];

            var accessToken =  _tokenService.CreateAccessToken(storedToken.User.Id, storedToken.User.Email, roles);

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            storedToken.IsRevoked = true;
            var newTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = storedToken.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
            _db.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = storedToken.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });
            await _db.SaveChangesAsync();

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };
        }

    }
}