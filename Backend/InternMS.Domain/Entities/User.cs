using System;
using System.Collections.Generic;

namespace InternMS.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UserProfile? Profile { get; set;}
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Project> CreateProjects { get; set; } = new List<Project>();
        public ICollection<ProjectTask> CreateTasks { get; set; } = new List<ProjectTask>();
    }
}