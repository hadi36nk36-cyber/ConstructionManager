namespace ConstructionManager.Api.Domain.Entities
{
    public class PurchaseInvoiceDetail
    {
        public int Id { get; set; }
        public int PurchaseInvoiceId { get; set; }
        public string MaterialName { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public PurchaseInvoice PurchaseInvoice { get; set; }
    }
}