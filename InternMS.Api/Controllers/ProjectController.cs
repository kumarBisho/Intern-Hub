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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue("id"));
        }

        private string GetUserRole() =>
            User.FindFirstValue(ClaimTypes.Role) ?? "Intern";

        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            var userId = GetUserId();
            var project = await _projectService.CreateProjectAsync(userId, dto);

            return Ok(_mapper.Map<ProjectDto>(project));
        }

        [Authorize(Roles = "Admin,Mentor")]
        [HttpPost("{projectId}/assign")]
        public async Task<IActionResult> Assign(Guid projectId, [FromBody] AssignProjectDto dto)
        {
            await _projectService.AssignProjectAsync(projectId, dto);
            return Ok(new { message = "Project assigned successfully." });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var projects = await _projectService.GetProjectsForUserAsync(userId, role);
            return Ok(_mapper.Map<IEnumerable<ProjectDto>>(projects));
        }

        [Authorize]
        [HttpGet("{id}")]

        public async Task<IActionResult> GetProject(Guid id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null) return NotFound();

            return Ok(project);
        }

        [Authorize(Roles = "Admin,Mentor,Intern")]
        [HttpPost("{projectId}/update")]
        public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] CreateProjectUpdateDto dto)
        {
            var userId = GetUserId();
            var update = await _projectService.AddProjectUpdateAsync(projectId, userId, dto);
            return Ok(update);
        }
    }
}