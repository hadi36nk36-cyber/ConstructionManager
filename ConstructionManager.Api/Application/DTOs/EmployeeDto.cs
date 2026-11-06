using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Address { get; set; }
        public DateTime BirthDate { get; set; }
        public string EmployeeCode { get; set; }
        public string Speciality { get; set; }
        public int? ExperienceYears { get; set; }
        public decimal BaseSalary { get; set; }
        public EmployeeStatus Status { get; set; }
    }
}