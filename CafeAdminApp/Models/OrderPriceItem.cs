using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CafeAdminApp.Models
{
    [Table("orderprice")]
    public class OrderPriceItem
    {
        [Column("orderid")]
        public int OrderId { get; set; }

        [Column("priceid")]
        public int PriceId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        
        
        [ValidateNever]
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        [ValidateNever]
        [ForeignKey(nameof(PriceId))]
        public Price Price { get; set; }
    }
}
