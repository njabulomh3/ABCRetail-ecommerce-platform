using ABCRetail_Cloud_.Models;
using ABCRetail_Cloud_.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail_Cloud_.Controllers
{
    public class OrderController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly QueueService _queueService;

        public OrderController(TableStorageService tableStorageService, QueueService queueService)
        {
            _tableStorageService = tableStorageService;
            _queueService = queueService;
        }

        // Action to display all orders
        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllOrdersAsync();
            return View(orders);
        }

        // Display the Create Order form
        public async Task<IActionResult> AddOrder()
        {
            var customers = await _tableStorageService.GetAllCustomerProfilesAsync();  
            var products = await _tableStorageService.GetAllProductsAsync();

            if (customers == null || customers.Count == 0)
            {
                ModelState.AddModelError("", "No customers found. Please add customers first.");
                return View();
            }

            if (products == null || products.Count == 0)
            {
                ModelState.AddModelError("", "No products found. Please add products first.");
                return View();
            }

            ViewData["Customers"] = customers;
            ViewData["Products"] = products;

            return View();
        }

        // Handle the Create Order form submission
        [HttpPost]
        public async Task<IActionResult> AddOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                order.Order_Date = DateTime.SpecifyKind(order.Order_Date, DateTimeKind.Utc);
                order.PartitionKey = "OrdersPartition";
                order.RowKey = Guid.NewGuid().ToString();
                await _tableStorageService.AddOrderAsync(order);

                // MessageQueue
                string message = $"New order by Customer {order.Customer_ID} for Product {order.Product_Name} at {order.Shipping_Address} on {order.Order_Date}";
                await _queueService.SendMessageAsync(message);

                return RedirectToAction("Index");
            }

            // Re-load the customers and products in case of form error
            var customers = await _tableStorageService.GetAllCustomerProfilesAsync();  // Fix for customer profile fetching
            var products = await _tableStorageService.GetAllProductsAsync();
            ViewData["Customers"] = customers;
            ViewData["Products"] = products;

            return View(order);
        }
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            try
            {
                await _tableStorageService.DeleteOrderAsync(partitionKey, rowKey);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log error (optional)
                ModelState.AddModelError("", $"Unable to delete order: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

    }
}
