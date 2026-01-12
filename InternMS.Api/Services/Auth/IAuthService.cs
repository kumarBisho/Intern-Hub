using System;
using System.Threading.Tasks;
using InternMS.Domain.Entities;

namespace InternMS.Api.Services.Auth
{
    public interface IAuthService
    {
        Task<User?> ValidateCredentialsAsync(string email, string password);
        Task<User?> RegisterUserAsync(string email, string password, string firstName, string lastName, int roleId);
    }
}