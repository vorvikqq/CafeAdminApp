using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CafeAdminApp.Models
{
    [Table("stock")]
    public class StockItem
    {
        [Key]
        [Column("stockid")]
        public int StockId { get; set; } = default!;

        [Column("productid")]
        public int ProductId { get; set; }
 
        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("isprosrochka")]
        public bool IsProsrochka { get; set; }


        [ValidateNever]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

    }
}
