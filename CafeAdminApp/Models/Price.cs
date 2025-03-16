using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CafeAdminApp.Models
{
    [Table("prices")]
    public class Price
    {
        [Key]
        [Column("priceid")]
        public int PriceId { get; set; } = default!;

        [Column("date")]
        public DateTimeOffset Date { get; set; }
        [Column("boughtprice")]
        public double BoughtPrice { get; set; }
        [Column("sellprice")]
        public double SellPrice { get; set; }

        [Column("productid")]
        public int ProductId { get; set; }
        
        [ValidateNever]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        public List<OrderPriceItem> OrderPrices { get; set; } = new List<OrderPriceItem>();
        public List<InvoicePriceItem> InvoicePrices { get; set; } = new List<InvoicePriceItem>();


    }
}
