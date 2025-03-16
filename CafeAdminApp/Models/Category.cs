using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeAdminApp.Models
{
    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("categoryid")]
        public int CategoryId { get; set; } = default!;

        [Column("categoryname")]
        public string CategoryName { get; set; }
    }
}
