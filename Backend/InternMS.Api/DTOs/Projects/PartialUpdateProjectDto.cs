namespace InternMS.Api.DTOs.Projects
{
    public class PartialUpdateProjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}