using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        /// <param name="productRepository">Repository for product-related data operations.</param>
        /// <param name="categoryRepository">Repository for category-related data operations.</param>
        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepo = productRepository;
            _categoryRepo = categoryRepository;
        }

        /// <summary>
        /// Displays a list of all products.
        /// </summary>
        /// <returns>The view with the product list.</returns>
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "ManageProducts";
            return View(await _productRepo.GetAllAsync());
        }

        /// <summary>
        /// Displays details of a specific product.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The view with product details or NotFound if not found.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepo.GetByIdAsync(id.Value);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Displays the form to create a new product.
        /// </summary>
        /// <returns>The create product view.</returns>
        public async Task<IActionResult> Create()
        {
            ViewData["Category"] = new SelectList(await _categoryRepo.GetAllCategoriesAsync(), "CategoryId", "CategoryName");
            return View();
        }

        /// <summary>
        /// Handles the form submission for creating a new product.
        /// </summary>
        /// <param name="product">The product model to create.</param>
        /// <returns>Redirects to Index if successful, otherwise redisplays the form.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ManufactureDate,ConsumptionDate,ProductName,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepo.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            ViewData["Category"] = new SelectList(await _categoryRepo.GetAllCategoriesAsync(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        /// <summary>
        /// Displays the form to edit an existing product.
        /// </summary>
        /// <param name="id">The ID of the product to edit.</param>
        /// <returns>The edit product view or NotFound if not found.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepo.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["Category"] = new SelectList(await _categoryRepo.GetAllCategoriesAsync(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        /// <summary>
        /// Handles the form submission for editing an existing product.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <param name="product">The updated product model.</param>
        /// <returns>Redirects to Index if successful, otherwise redisplays the form.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ManufactureDate,ConsumptionDate,ProductName,CategoryId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _productRepo.UpdateAsync(id, product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["Category"] = new SelectList(await _categoryRepo.GetAllCategoriesAsync(), "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        /// <summary>
        /// Displays the confirmation view for deleting a product.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>The delete confirmation view or NotFound if not found.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepo.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// <summary>
        /// Handles the confirmed deletion of a product.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>Redirects to Index after deletion.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a product with the given ID exists.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <returns>True if product exists, otherwise false.</returns>
        private bool ProductExists(int id)
        {
            return _productRepo.IsExist(id);
        }
    }

}
