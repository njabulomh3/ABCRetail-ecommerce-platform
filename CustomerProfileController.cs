using ABCRetail_Cloud_.Models;
using ABCRetail_Cloud_.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail_Cloud_.Controllers
{
    public class CustomerProfileController : Controller
    {
        private readonly TableStorageService _tableStorageService;

        public CustomerProfileController(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        public async Task<IActionResult> Index()
        {
            var customerProfiles = await _tableStorageService.GetAllCustomerProfilesAsync();
            return View(customerProfiles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerProfile customerProfile)
        {
            customerProfile.PartitionKey = "CustomerProfilesPartition";
            customerProfile.RowKey = Guid.NewGuid().ToString();

            await _tableStorageService.AddCustomerProfileAsync(customerProfile);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await _tableStorageService.DeleteCustomerProfileAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var customerProfile = await _tableStorageService.GetCustomerProfileAsync(partitionKey, rowKey);
            if (customerProfile == null)
            {
                return NotFound();
            }
            return View(customerProfile);
        }
    }

}

