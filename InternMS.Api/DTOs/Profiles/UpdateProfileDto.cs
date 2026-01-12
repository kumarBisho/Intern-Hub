namespace InternMS.Api.DTOs.Profiles
{
    public class UpdateProfileDto
    {
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? Bio { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}