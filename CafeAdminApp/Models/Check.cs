using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CafeAdminApp.Models
{
    [Table("checks")]
    public class Check
    {
        [Key]
        [Column("checkid")]
        public int CheckId { get; set; } = default!;

        [Column("saledate")]
        public DateTimeOffset SaleDate { get; set; }

        [Column("orderid")]
        public int OrderId { get; set; }


        [ValidateNever]
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
    }
}
