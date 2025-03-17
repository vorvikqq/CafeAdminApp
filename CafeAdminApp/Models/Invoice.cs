using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeAdminApp.Models
{
    [Table("invoices")]
    public class Invoice
    {
        [Key]
        [Column("invoiceid")]
        public int InvoiceId { get; set; } = default!;

        [Column("createdate")]
        public DateTimeOffset CreateDate { get; set; }

        [ValidateNever]
        public List<InvoicePriceItem> InvoicePrices { get; set; } = new List<InvoicePriceItem>();

    }
}
