using AutoMapper;
using AutoMapper.QueryableExtensions;
using InternMS.Api.DTOs.Tasks;
using InternMS.Domain.Entities;
using InternMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InternMS.Api.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public TaskService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;   
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto dto)
        {
            var projectExists = await _db.Projects.AnyAsync(p => p.Id == dto.ProjectId);
            if (!projectExists)
                throw new KeyNotFoundException("Project not found.");

            var task = _mapper.Map<ProjectTask>(dto);
            _db.ProjectTasks.Add(task);
            await _db.SaveChangesAsync();

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<IEnumerable<TaskListDto>> GetTasksByProjectAsync(Guid projectId)
        {
            return await _db.ProjectTasks
                .Where(t => t.ProjectId == projectId)
                .OrderBy(t => t.EndDate)
                .ProjectTo<TaskListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TaskDto?> GetTaskByIdAsync(Guid taskId)
        {
            var task = await _db.ProjectTasks
                .FirstOrDefaultAsync(t => t.Id == taskId);

            return task == null ? null : _mapper.Map<TaskDto>(task);
        }

        public async Task UpdateTaskAsync(Guid taskId, UpdateTaskDto dto)
        {
            var task = await _db.ProjectTasks.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            _mapper.Map(dto, task);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(Guid taskId)
        {
            var task = await _db.ProjectTasks.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            _db.ProjectTasks.Remove(task);
            await _db.SaveChangesAsync();
        }
    }
}