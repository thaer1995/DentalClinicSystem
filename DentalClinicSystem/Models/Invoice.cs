namespace DentalClinicSystem.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount => TotalAmount - PaidAmount;
        public ICollection<InvoiceItem> Items { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}
