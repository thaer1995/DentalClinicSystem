using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DentalClinicSystem.ViewModels
{
    public class InvoiceViewModel
    {
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "رقم المريض مطلوب")]
        public int PatientId { get; set; }

        [Display(Name = "المبلغ المدفوع")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون المبلغ المدفوع أكبر من أو يساوي صفر")]
        public decimal PaidAmount { get; set; }

        [Required(ErrorMessage = "يجب إضافة عنصر واحد على الأقل")]
        public List<InvoiceItemViewModel> Items { get; set; } = new List<InvoiceItemViewModel>();
    }

    public class InvoiceItemViewModel
    {
        [Required(ErrorMessage = "يجب اختيار العلاج")]
        [Display(Name = "العلاج")]
        public int TreatmentId { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون السعر أكبر من صفر")]
        [Display(Name = "السعر")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب أن تكون الكمية أكبر من صفر")]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }
    }

    public class UpdatePaymentViewModel
    {
        public int InvoiceId { get; set; }

        [Display(Name = "المبلغ المدفوع حالياً")]
        public decimal CurrentPaidAmount { get; set; }

        [Display(Name = "المبلغ الإجمالي")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "المبلغ المتبقي")]
        public decimal RemainingAmount { get; set; }

        [Required(ErrorMessage = "المبلغ الإضافي مطلوب")]
        [Range(0, double.MaxValue, ErrorMessage = "يجب أن يكون المبلغ الإضافي أكبر من أو يساوي صفر")]
        [Display(Name = "المبلغ الإضافي")]
        public decimal AdditionalPayment { get; set; }
    }

    public class InvoiceListViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string PaymentStatus { get; set; }
    }
}