using System;
using System.ComponentModel.DataAnnotations;
using ABCRetail_Cloud_.Models;
using System.Collections.Generic;
using ABCRetail_Cloud_.Models.Sql;

namespace ABCRetail_Cloud_.Models
{
    public enum OrderStatus
    {
        New,
        Processed,
        Cancelled
    }

    public class OrderSql
    {
        [Key]
        public int Id { get; set; }                 // PK
        public string UserId { get; set; }          // Identity user
        public int CustomerId { get; set; }         // optional link to CustomerSql (if separate)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public string ShippingAddress { get; set; }
        public decimal Total { get; set; }

        // Use a JSON payload or separate OrderItems table; here we'll create OrderItem table below
        public List<OrderItemSql> Items { get; set; } = new List<OrderItemSql>();
    }

    public class OrderItemSql
    {
        [Key]
        public int Id { get; set; }
        public int OrderSqlId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public ProductSql Product { get; set; }
    }
}
