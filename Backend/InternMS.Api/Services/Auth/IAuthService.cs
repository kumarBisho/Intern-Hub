using System;
using System.Threading.Tasks;
using InternMS.Domain.Entities;

namespace InternMS.Api.Services.Auth
{
    public interface IAuthService
    {
        Task<User?> ValidateCredentialsAsync(string email, string password);
        Task<User?> ConfirmEmailAsync(string token);
        Task<bool> ApproveUserAsync(Guid userId);
        Task<User?> RegisterUserAsync(string email, string password, string firstName, string lastName, int roleId);
        Task LogoutAsync(string token);
        Task<bool> IsTokenBlacklistedAsync(string token);

    }
}