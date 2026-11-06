using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int CustomerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public ProjectStatus Status { get; set; } 
        public string Location { get; set; }
        public string Description { get; set; }

        public Customer Customer { get; set; }
        public ICollection<ProjectAssignment> ProjectAssignments { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
    }
}