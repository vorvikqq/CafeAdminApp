using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CafeAdminApp.Models
{
    [Table("invoiceprice")]
    public class InvoicePriceItem
    {
        [Column("priceid")]
        public int PriceId { get; set; }
        
        [Column("invoiceid")]
        public int InvoiceId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        
        [ValidateNever]
        [ForeignKey(nameof(PriceId))]
        public Price Price { get; set; }

        [ValidateNever]
        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; }
    }
}
