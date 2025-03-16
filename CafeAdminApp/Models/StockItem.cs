using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CafeAdminApp.Models
{
    [Table("Stock")]
    public class StockItem
    {
        [Key]
        [Column("StockID")]
        public int StockId { get; set; } = default!;

        [Column("ProductID")]
        public int ProductId { get; set; }
        
        public int Quantity { get; set; }
        
        public bool IsProsrochka { get; set; }


        [ValidateNever]
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

    }
}
