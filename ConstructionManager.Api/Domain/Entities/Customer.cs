using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public CustomerType CustomerType { get; set; } 
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Address { get; set; }

        public ICollection<Project> Projects { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
    }
}