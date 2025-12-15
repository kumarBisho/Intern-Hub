using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternMS.Api.Services;
using InternMS.Api.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace InternMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public ProfileController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        // GET api/profile/me
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = Guid.Parse(User.FindFirstValue("id"));
            var profile = await _userService.GetProfileAsync(userId);

            if (profile == null)
                return Ok(new ProfileDto()); // empty profile for new users

            return Ok(_mapper.Map<ProfileDto>(profile));
        }

        // PUT api/profile/me
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue("id"));
            await _userService.UpdateProfileAsync(userId, dto);
            return NoContent();
        }

        // Admin can view any profile
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