using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternMS.Api.Services.Tasks;
using InternMS.Api.DTOs.Tasks;
using System.Security.Claims;

namespace InternMS.Api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            var task = await _taskService.CreateTaskAsync(dto);
            return CreatedAtAction(nameof(GetTaskById), new { taskId = task.Id }, task);
        }

        [HttpGet("{taskId:guid}")]
        public async Task<IActionResult> GetTaskById(Guid taskId)
        {
            var task = await _taskService.GetTaskByIdAsync(taskId);
            if(task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpGet("project/{projectId:guid}")]
        public async Task<IActionResult> GetTasksByProject(Guid projectId)
        {
            var tasks = await _taskService.GetTasksByProjectAsync(projectId);
            return Ok(tasks);
        }

        [HttpPut("{taskId:guid}")]
        [Authorize(Roles = "Admin,Mentor")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskDto dto)
        {
            await _taskService.UpdateTaskAsync(taskId, dto);
            return NoContent();
        }

        [HttpDelete("{taskId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            await _taskService.DeleteTaskAsync(taskId);
            return NoContent();
        }
    }
}