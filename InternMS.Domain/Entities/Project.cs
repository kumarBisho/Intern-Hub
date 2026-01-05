using System;
using System.Collections.Generic;

namespace InternMS.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = default!;

        public ICollection<ProjectAssignment> Assignments { get; set; } = new List<ProjectAssignment>();
        public ICollection<ProjectUpdate> Updates { get; set; } = new List<ProjectUpdate>();
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }

}