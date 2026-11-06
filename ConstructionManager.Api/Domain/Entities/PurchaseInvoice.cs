using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Domain.Entities
{
    public class PurchaseInvoice
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string TrackingCode { get; set; }
        public int InvoiceId { get; set; }
        public int ProjectId { get; set; }
        public int SupplierId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public PurchaseInvoiceStatus Status { get; set; }

        public Invoice Invoice { get; set; }
        public Project Project { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<PurchaseInvoiceDetail> Details { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}