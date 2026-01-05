using ABCRetail_Cloud_.Areas.Identity.Data;
using ABCRetail_Cloud_.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ABCRetail_Cloud_.Services
{
    public class CartService
    {
        private readonly ABCRetail_Cloud_Context _db;
        private readonly QueueService _queue;

        public CartService(ABCRetail_Cloud_Context db, QueueService queue)
        {
            _db = db;
            _queue = queue;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity = 1)
        {
            var item = await _db.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
            if (item == null)
            {
                item = new CartItem { UserId = userId, ProductId = productId, Quantity = quantity };
                _db.CartItems.Add(item);
            }
            else
            {
                item.Quantity += quantity;
                _db.CartItems.Update(item);
            }
            await _db.SaveChangesAsync();
        }

        public async Task<List<CartItem>> GetCartAsync(string userId)
        {
            return await _db.CartItems
                        .Include(ci => ci.Product)
                        .Where(ci => ci.UserId == userId)
                        .ToListAsync();
        }

        public async Task RemoveFromCartAsync(string userId, int cartItemId)
        {
            var item = await _db.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
            if (item != null)
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<OrderSql> CheckoutAsync(string userId, string shippingAddress)
        {
            var cartItems = await GetCartAsync(userId);
            if (cartItems == null || !cartItems.Any()) return null;

            var order = new OrderSql
            {
                UserId = userId,
                ShippingAddress = shippingAddress,
                CreatedAt = DateTime.UtcNow
            };

            decimal total = 0m;
            foreach (var ci in cartItems)
            {
                var unitPrice = ci.Product?.Price ?? 0; // ProductSql.Price is decimal
                var oi = new OrderItemSql
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = unitPrice
                };
                order.Items.Add(oi);
                total += unitPrice * ci.Quantity;

                // Optional: decrement product stock
                var prod = await _db.ProductsSql.FindAsync(ci.ProductId);
                if (prod != null)
                {
                    prod.Stock = Math.Max(0, prod.Stock - ci.Quantity);
                    _db.ProductsSql.Update(prod);
                }
            }
            order.Total = total;

            _db.OrdersSql.Add(order);

            // Remove cart items
            _db.CartItems.RemoveRange(cartItems);

            await _db.SaveChangesAsync();

            // Enqueue message (simple JSON)
            var message = System.Text.Json.JsonSerializer.Serialize(new
            {
                OrderId = order.Id,
                UserId = order.UserId,
                Total = order.Total,
                Items = order.Items.Select(i => new { i.ProductId, i.Quantity, i.UnitPrice })
            });

            await _queue.SendMessageAsync(message);

            return order;
        }
    }
}

