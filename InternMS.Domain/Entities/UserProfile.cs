using System;

namespace InternMS.Domain.Entities
{
    public class UserProfile
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? Bio { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}