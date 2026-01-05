using InternMS.Api.DTOs.Tasks;

namespace InternMS.Api.Services.Tasks
{
    public interface ITaskService
    {
        Task<TaskDto> CreateTaskAsync(CreateTaskDto dto);
        Task<IEnumerable<TaskListDto>> GetTasksByProjectAsync(Guid projectId);
        Task<TaskDto?> GetTaskByIdAsync(Guid taskId);
        Task UpdateTaskAsync(Guid taskId, UpdateTaskDto dto);

        Task DeleteTaskAsync(Guid taskId);
    }
}