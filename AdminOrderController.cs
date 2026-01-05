using ABCRetail_Cloud_.Areas.Identity.Data;
using ABCRetail_Cloud_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ABCRetail_Cloud_.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly ABCRetail_Cloud_Context _db;

        public AdminOrderController(ABCRetail_Cloud_Context db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _db.OrdersSql
                                 .Include(o => o.Items)
                                 .ToListAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
        {
            var order = await _db.OrdersSql.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                _db.OrdersSql.Update(order);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
