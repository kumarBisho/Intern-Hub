using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using InternMS.Api.DTOs;
using InternMS.Api.Auth;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Linq;


namespace InternMS.Api.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;


        public AuthController(IAuthService authService, ITokenService tokenService, IMapper mapper)
        {
            _authService = authService;
            _tokenService = tokenService;
            _mapper = mapper;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var user = await _authService.ValidateCredentialsAsync(request.Email, request.Password);
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });


            var roles = user.UserRoles?.Select(ur => ur.Role.Name) ?? new string[0];
            var token = _tokenService.CreateToken(user.Id, user.Email, roles);


            var userDto = _mapper.Map<UserDto>(user);
            var response = new LoginResponseDto { Token = token, User = userDto };
            return Ok(response);
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
    }
}