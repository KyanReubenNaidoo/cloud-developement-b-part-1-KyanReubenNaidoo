using POECLDV6212.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using POECLDV6212.Models;

namespace POECLDV6212.Controllers
{
    public class ProductController : Controller
    {
        private readonly Blob_Service _blob;
        private readonly Table_Service _tableStorage;

        public ProductController(Blob_Service blob_Service, Table_Service table_Service)
        {
            _blob = blob_Service;
            _tableStorage = table_Service;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _tableStorage.GetAllProductsAsync();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile file)
        {
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                var image = await _blob.UploadsAsync(stream, file.FileName);
                product.Image = image;
            }

            if (ModelState.IsValid)
            {
                product.PartitionKey = "ProductPartitionKey";
                product.RowKey = Guid.NewGuid().ToString();
                product.Product_ID++;
                await _tableStorage.AddProductAsync(product);
                return RedirectToAction("Index");
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string partitionKey, string rowKey, Product product)
        {
            if (product != null && !string.IsNullOrEmpty(product.Image))
            {
                await _blob.DeleteBlobAsync(product.Image);
            }
            await _tableStorage.DeleteProductAsync(partitionKey, rowKey);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile file)
        {
            if (file != null)
            {
                var existingProduct = await _tableStorage.ProductDetailsAsync(product.PartitionKey, product.RowKey);

                // Delete old blob if it exists
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    await _blob.DeleteBlobAsync(existingProduct.Image);
                }

                // Upload new blob
                using var stream = file.OpenReadStream();
                var image = await _blob.UploadsAsync(stream, file.FileName);
                product.Image = image;
            }

            if (ModelState.IsValid)
            {
                await _tableStorage.UpdateProductAsync(product);
                return RedirectToAction("Index");
            }

            return View(product);
        }


        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string partitionKey, string rowKey)
        {
            var product = await _tableStorage.ProductDetailsAsync(partitionKey, rowKey);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var product = await _tableStorage.ProductDetailsAsync(partitionKey, rowKey);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }



    }
}