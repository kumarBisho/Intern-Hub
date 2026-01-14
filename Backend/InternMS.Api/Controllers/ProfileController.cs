using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternMS.Api.Services.Users;
using InternMS.Api.DTOs.Profiles;
using AutoMapper;
using System.Security.Claims;

namespace InternMS.Api.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize] // applies to all endpoints
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ProfileController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(id, out var userId))
                return Unauthorized("Invalid or missing user ID in token.");

            var profile = await _userService.GetProfileAsync(userId);

            if (profile == null)
                return Ok(new ProfileDto());

            return Ok(_mapper.Map<ProfileDto>(profile));
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(id, out var userId))
                return Unauthorized("Invalid or missing user ID in token.");

            await _userService.UpdateProfileAsync(userId, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            var profile = await _userService.GetProfileAsync(userId);
            if (profile == null) return NotFound();

            return Ok(_mapper.Map<ProfileDto>(profile));
        }
    }
}
