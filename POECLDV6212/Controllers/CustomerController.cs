using POECLDV6212.Models;
using POECLDV6212.Services;
using Microsoft.AspNetCore.Mvc;


namespace POECLDV6212.Controllers
{
    public class CustomerController : Controller
    {
        private readonly Table_Service _tableStorage;
        public CustomerController(Table_Service tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _tableStorage.GetAllCustomersAsync();
            return View(customers);
        }
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var customer = await _tableStorage.CustomerDetailsAsync(partitionKey, rowKey);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            await _tableStorage.DeleteCustomerAsync(partitionKey, rowKey);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(POECLDV6212.Models.Customer customer)
        {
            customer.PartitionKey = "CustomersPartition";
            customer.RowKey = Guid.NewGuid().ToString();
            customer.Customer_ID++;
            await _tableStorage.AddCustomerAsync(customer);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult AddCustomer()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            var customer = await _tableStorage.CustomerDetailsAsync(partitionKey, rowKey);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Customer updatedCustomer)
        {
            await _tableStorage.UpdateCustomerAsync(updatedCustomer);
            return RedirectToAction("Index");
        }

    }
}
