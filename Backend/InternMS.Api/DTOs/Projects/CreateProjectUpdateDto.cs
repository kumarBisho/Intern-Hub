namespace InternMS.Api.DTOs.Projects
{
    public class CreateProjectUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Comment { get; set; }
    }
}