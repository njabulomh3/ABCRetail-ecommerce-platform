using System.ComponentModel.DataAnnotations;

namespace ABCRetail_Cloud_.Models.Sql
{
    public class ProductSql
    {
        [Key]
        public int Id { get; set; }              // PK - for SQL use
        [Required]
        public string SKU { get; set; }          // unique SKU or generated code
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }     // Blob URL
        public int Stock { get; set; }
        // Optionally reference Table storage entry
        public string TablePartitionKey { get; set; }
        public string TableRowKey { get; set; }
    }
}
