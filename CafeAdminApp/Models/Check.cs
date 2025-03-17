using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CafeAdminApp.Models
{
    [Table("Checks")]
    public class Check
    {
        [Key]
        [Column("CheckID")]
        public int CheckId { get; set; } = default!;

        public DateTimeOffset SaleDate { get; set; }

        [Column("OrderID")]
        public int OrderId { get; set; }


        [ValidateNever]
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }
    }
}
