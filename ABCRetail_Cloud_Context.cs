using ABCRetail_Cloud_.Models;
using ABCRetail_Cloud_.Models.Sql;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ABCRetail_Cloud_.Areas.Identity.Data
{
    public class ABCRetail_Cloud_Context : IdentityDbContext<ABCRetail_Cloud_User>
    {
        public ABCRetail_Cloud_Context(DbContextOptions<ABCRetail_Cloud_Context> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure decimal precision to avoid truncation warnings
            builder.Entity<OrderItemSql>()
                .Property(o => o.UnitPrice)
                .HasPrecision(18, 2); // 18 total digits, 2 decimal places

            builder.Entity<OrderSql>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            builder.Entity<ProductSql>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // Additional relationship config
            builder.Entity<OrderItemSql>()
                   .HasOne(i => i.Product)
                   .WithMany()
                   .HasForeignKey(i => i.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderSql>()
                   .HasMany(o => o.Items)
                   .WithOne()
                   .HasForeignKey(i => i.OrderSqlId)
                   .OnDelete(DeleteBehavior.Cascade);
        }

        // SQL tables for POE Part 3
        public DbSet<CustomerSql> CustomersSql { get; set; }
        public DbSet<ProductSql> ProductsSql { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderSql> OrdersSql { get; set; }
        public DbSet<OrderItemSql> OrderItemsSql { get; set; }
    }
}