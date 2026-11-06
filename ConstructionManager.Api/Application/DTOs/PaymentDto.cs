using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string TrackingCode { get; set; }
        public int? PurchaseInvoiceId { get; set; }
        public int? EmployeeId { get; set; }
        public int? EquipmentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentType PaymentType { get; set; }
        public string Description { get; set; }
    }
}