namespace ConstructionManager.Api.Domain.Entities
{
    public class ProjectAssignment
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string Role { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Project Project { get; set; }
        public Employee Employee { get; set; }
    }
}