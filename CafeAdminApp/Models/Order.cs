using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeAdminApp.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("orderid")]
        public int OrderId { get; set; } = default!;

        [Column("orderstatus")]
        public bool OrderStatus { get; set; }
        [Column("orderdate")]
        public DateTimeOffset OrderDate { get; set; }

        [ValidateNever]
        public List<OrderPriceItem> OrderPrices { get; set; } = new List<OrderPriceItem>();
    }
}
