
using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;
namespace ABCRetail_Cloud_.Models
{


    public class CustomerProfile : ITableEntity
    {
        [Key]
        public int Customer_Id { get; set; }
        public string? Customer_Name { get; set; }
        public string? Email { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }

}
