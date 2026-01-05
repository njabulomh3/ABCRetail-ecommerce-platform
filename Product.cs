
using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;
namespace ABCRetail_Cloud_.Models
{


    public class Product : ITableEntity
    {
        [Key]
  
        public string? Product_Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}

