using ABCRetail_Cloud_.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ABCRetail_Cloud_.Areas.Identity.Data;
using System.Linq;

namespace ABCRetail_Cloud_.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly UserManager<ABCRetail_Cloud_User> _userManager;

        public CartController(CartService cartService, UserManager<ABCRetail_Cloud_User> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int qty = 1)
        {
            var userId = _userManager.GetUserId(User);
            await _cartService.AddToCartAsync(userId, productId, qty);
            TempData["Message"] = "Added to cart.";
            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var items = await _cartService.GetCartAsync(userId);
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var userId = _userManager.GetUserId(User);
            await _cartService.RemoveFromCartAsync(userId, cartItemId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string shippingAddress)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _cartService.CheckoutAsync(userId, shippingAddress);
            if (order == null)
            {
                TempData["Message"] = "Cart empty.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Message"] = $"Order placed (ID: {order.Id})";
            return RedirectToAction("OrderConfirmation", new { id = order.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            ViewData["OrderId"] = id;
            return View();
        }
    }
}

