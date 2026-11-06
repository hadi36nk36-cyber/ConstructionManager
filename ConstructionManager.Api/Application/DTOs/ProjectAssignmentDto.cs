namespace ConstructionManager.Api.Application.DTOs
{
    public class ProjectAssignmentDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string Role { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}