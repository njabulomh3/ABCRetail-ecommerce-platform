
using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ABCRetail_Cloud_.Models
{

    public class Order : ITableEntity
    {
        [Key]
        public int Order_Id { get; set; }

        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        [Required(ErrorMessage = "Please select a customer.")]
        public int Customer_ID { get; set; } // FK to the Customer who made the order

        [Required(ErrorMessage = "Please select a product.")]
        public string  Product_Name { get; set; } // FK to the Product being ordered

        [Required(ErrorMessage = "Please select the date.")]
        public DateTime Order_Date { get; set; }

        [Required(ErrorMessage = "Please enter the shipping address.")]
        public string? Shipping_Address { get; set; }
    }
}



