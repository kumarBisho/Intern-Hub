using InternMS.Api.DTOs;
using InternMS.Domain.Entities;

namespace InternMS.Api.Services
{
    public interface IProjectService
    {
        Task<Project> CreateProjectAsync(Guid creatorId, CreateProjectDto dto);
        Task<Project?> GetProjectByIdAsync(Guid id);
        Task<IEnumerable<Project>> GetProjectsForUserAsync(Guid userId, string role);
        Task AssignProjectAsync(Guid projectId, AssignProjectDto dto);
        Task<ProjectUpdateDto> AddProjectUpdateAsync(Guid projectId, Guid authorId, CreateProjectUpdateDto dto);
    }
}