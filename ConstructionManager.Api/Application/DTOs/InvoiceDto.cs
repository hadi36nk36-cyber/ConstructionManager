using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string TrackingCode { get; set; }
        public int ProjectId { get; set; }
        public int CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}