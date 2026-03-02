using Microsoft.AspNetCore.Mvc;
using InternMS.Api.DTOs.Authentication;
using InternMS.Api.DTOs.Users;
using InternMS.Api.Services.Auth;
using InternMS.Api.Services.Token;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using InternMS.Api.Middleware;
using InternMS.Domain.Entities;
using InternMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternMS.Api.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    // [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;


        public AuthController(IAuthService authService, ITokenService tokenService, IMapper mapper, AppDbContext context)
        {
            _authService = authService;
            _tokenService = tokenService;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var user = await _authService.ConfirmEmailAsync(token);
            if(user == null) return BadRequest(new { message = "Invalid or expired token." });
            return Ok(new { message = "Email confirmed successfully. You can now log in." });
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet("approve-user")]
        public async Task<IActionResult> ApproveUser(Guid userId)
        {
            var result = await _authService.ApproveUserAsync(userId);
            if (!result) return BadRequest(new { message = "User not found or already approved." });
            return Ok(new { message = "User approved successfully." });
        }

        // Register - restrict to Admin role (change as needed)
        
        [HttpPost("register")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto request)
        {
            try
            {
            var user = await _authService.RegisterUserAsync(request.Email, request.Password, request.FirstName, request.LastName, request.RoleId);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
            }
            catch (System.Exception ex)
            {
            return BadRequest(new { message = ex.Message });
            }
        }


        // Login User
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var user = await _authService.ValidateCredentialsAsync(request.Email, request.Password);
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });


            var roles = user.UserRoles?.Select(ur => ur.Role.Name) ?? new string[0];
            var accessToken = _tokenService.CreateAccessToken(user.Id, user.Email, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var RefreshToken = new RefreshToken{
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7) // Set refresh token expiry (configurable)
            };

            _context.RefreshTokens.Add(RefreshToken);
             await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            var response = new LoginResponseDto { AccessToken = accessToken, RefreshToken = refreshToken, User = userDto };
            return Ok(response);
        }


        // Logout User
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (!authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new { message = "Invalid token format." });
            }

            var token = authHeader.Replace("Bearer ", "");
            await _authService.LogoutAsync(token);
            return Ok(new { message = "Successfully logged out." });
        }

        // Refresh Token Endpoint
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (result == null) return BadRequest(new { message = "Invalid or expired refresh token." });
            return Ok(result);
        }
    }
}