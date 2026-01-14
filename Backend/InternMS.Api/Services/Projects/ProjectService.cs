using AutoMapper;
using InternMS.Api.Services;
using InternMS.Infrastructure.Data;
using InternMS.Domain.Entities;
using InternMS.Api.DTOs.Projects;
using Microsoft.EntityFrameworkCore;

namespace InternMS.Api.Services.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public ProjectService(AppDbContext db, INotificationService notificationService, IMapper mapper)
        {
            _db = db;
             _notificationService = notificationService;
             _mapper = mapper;
        }

        public async Task<Project> CreateProjectAsync(Guid creatorId, CreateProjectDto dto)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate.HasValue 
                    ? DateTime.SpecifyKind(dto.StartDate.Value, DateTimeKind.Utc) 
                    : null,
                EndDate = dto.EndDate.HasValue 
                    ? DateTime.SpecifyKind(dto.EndDate.Value, DateTimeKind.Utc) 
                    : null,
                CreatedById = creatorId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            return project;
        }
        public async Task<Project?> GetProjectByIdAsync(Guid id)
        {
            return await _db.Projects
                // .Include(p => p.Assignments).ThenInclude(a => a.Intern)
                // .Include(p => p.Assignments).ThenInclude(a => a.Mentor)
                // .Include(p => p.Updates).ThenInclude(u => u.Author)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetProjectsForUserAsync(Guid userId, string role)
        {
            if (role == "Admin")
            {
                return await _db.Projects
                    .Include(p => p.Assignments)
                    .ToListAsync();
            }
            
            if (role == "Mentor")
            {
                return await _db.Projects
                    .Include(p => p.Assignments)
                    .Where(p => p.Assignments.Any(a => a.MentorId == userId))
                    .ToListAsync();
            }

            // Intern
            return await _db.Projects
                .Include(p => p.Assignments)
                .Where(p => p.Assignments.Any(a => a.InternId == userId))
                .ToListAsync();
        }

        public async Task AssignProjectAsync(Guid projectId, AssignProjectDto dto)
        {
            var exists = await _db.ProjectAssignments
                .AnyAsync(a => a.ProjectId == projectId && a.InternId == dto.InternId);

            if (exists)
                throw new Exception("Intern is already assigned!");

            var assignment = new ProjectAssignment
            {
                ProjectId = projectId,
                InternId = dto.InternId,
                MentorId = dto.MentorId,
                AssignedAt = DateTime.UtcNow
            };

            _db.ProjectAssignments.Add(assignment);
            await _db.SaveChangesAsync();

            await _notificationService.CreateNotificationAsync(
                dto.InternId,
                "New Project Assigned",
                "A new project has been assigned to you."
            );
        }

        public async Task<ProjectUpdateDto> AddProjectUpdateAsync(Guid projectId, Guid authorId, CreateProjectUpdateDto dto)
        {
            var update = new ProjectUpdate
            {
                ProjectId = projectId,
                AuthorId = authorId,
                Status = Enum.Parse<ProjectStatus>(dto.Status),
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow  
            };

            _db.ProjectUpdates.Add(update);

            // Update project status
            var project = await _db.Projects
                .Include(p => p.Assignments)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new Exception("Project not found");

            project.Status = update.Status;

            await _db.SaveChangesAsync();

            // 🔎 Find mentor (if any assignment exists)
            var firstAssignment = project.Assignments.FirstOrDefault();
            if (firstAssignment?.MentorId != null)
            {
                // 🔔 Send notification to mentor
                await _notificationService.CreateNotificationAsync(
                    firstAssignment.MentorId,
                    "Project Updated",
                    "Your intern has updated the project status."
                );
            }

            return _mapper.Map<ProjectUpdateDto>(update);
        }
    }
}