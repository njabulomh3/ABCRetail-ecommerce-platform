using ABCRetail_Cloud_.Models.Sql;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail_Cloud_.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        // Link to Identity user
        [Required]
        public string UserId { get; set; }

        [Required]
        public int ProductId { get; set; }    // FK to ProductSql.Id

        public int Quantity { get; set; } = 1;

        [ForeignKey("ProductId")]
        public ProductSql Product { get; set; }
    }
}

