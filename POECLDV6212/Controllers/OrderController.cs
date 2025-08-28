using POECLDV6212.Models;
using POECLDV6212.Services;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace POECLDV6212.Controllers
{
    public class OrderController : Controller
    {
        private readonly Table_Service _table;
        private readonly QueueService _queue;

        public OrderController(Table_Service table, QueueService queue)
        {
            _table = table;
            _queue = queue;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _table.GetAllOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> AddOrder()
        {
            var customers = await _table.GetAllCustomersAsync();
            var products = await _table.GetAllProductsAsync();

            if (customers == null || customers.Count == 0)
            {
                ModelState.AddModelError("", "No Customers found. Please add customers first");
                return View();
            }
            if (products == null || products.Count == 0)
            {
                ModelState.AddModelError("", "No Products found. Please add products first");
                return View();
            }

            ViewData["Customer"] = customers;
            ViewData["Product"] = products;
            return View();
        }

        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            var order = await _table.OrderDetailsAsync(partitionKey, rowKey);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
                order.PartitionKey = "OrderPartitionKey";
                order.RowKey = Guid.NewGuid().ToString();
                order.Order_ID++;
                await _table.AddOrderAsync(order);

                //Message Queues
                string message = $"New Order by customer : {order.Order_ID}" + $" of the product : {order.Product_ID}" + $" on {order.OrderDate}" + $", regarding {order.Description}";
                string message2 = $"Processing the new order";
                await _queue.SendMessages(message);
                await _queue.SendMessages(message2);
                return RedirectToAction("Index");
            }

            var customers = await _table.GetAllCustomersAsync();
            var products = await _table.GetAllProductsAsync();
            ViewData["Customer"] = customers;
            ViewData["Product"] = products;
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey, Order order)
        {
            await _table.DeleteOrderAsync(partitionKey, rowKey);
            string message = $"Order deleted: {order.RowKey} (Customer: {order.Customer_ID}, Product: {order.Product_ID})";
            string message2 = $"The order placed on {order.OrderDate} with description '{order.Description}' has been removed.";
            await _queue.SendMessages(message);
            await _queue.SendMessages(message2);

            return RedirectToAction(nameof(Index));
        }
    }
}
