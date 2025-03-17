using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CafeAdminApp.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("productid")]
        public int ProductId { get; set; } = default!;

        [Column("manufacturedate")]
        public DateTimeOffset ManufactureDate { get; set; }
        
        [Column("consumptiondate")]
        public DateTimeOffset ConsumptionDate { get; set; }
        
        [Column("productname")]
        public string ProductName { get; set; }

        [Column("categoryid")]
        public int CategoryId { get; set; }


        [ValidateNever]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
    }
}
