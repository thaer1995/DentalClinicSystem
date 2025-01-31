namespace DentalClinicSystem.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public int TreatmentId { get; set; }
        public Treatment Treatment { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
    public enum PaymentStatus
    {
        Pending,
        PartiallyPaid,
        Paid
    }
}
