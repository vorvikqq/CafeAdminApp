using CafeAdminApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CafeAdminApp.Controllers
{
    public class ProductExpirationController : Controller
    {
        private readonly IStockRepository _stockRepo;
        private readonly IProductRepository _prodRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductExpirationController"/> class.
        /// </summary>
        /// <param name="stockRepository">Repository for stock operations.</param>
        /// <param name="productRepository">Repository for product data operations.</param>
        public ProductExpirationController(IStockRepository stockRepository, IProductRepository productRepository)
        {
            _stockRepo = stockRepository;
            _prodRepo = productRepository;
        }

        /// <summary>
        /// Displays the default view for product expiration management.
        /// </summary>
        /// <returns>The view for the product expiration page.</returns>
        public IActionResult Index()
        {
            ViewData["ActivePage"] = "ProductExpiration";

            return View();
        }

        /// <summary>
        /// Marks expired products by setting the IsProsporchka flag to true.
        /// </summary>
        /// <returns>The result view with a message about the number of expired products updated.</returns>
        [HttpPost]
        public async Task<IActionResult> SetExpirationFlag()
        {
            var expiredProducts = await _prodRepo.GetExpiredProductsAsync();

            if (!expiredProducts.Any())
            {
                ViewData["Message"] = "No expired products found.";
                ViewData["Count"] = 0;
                return View("Index");
            }

            var expiredProductIds = expiredProducts.Select(p => p.ProductId).ToList();

            var expiredCount = await _stockRepo.SetExpiredProductsAsync(expiredProductIds);

            ViewData["Message"] = $"Marked {expiredCount} products as expired.";
            ViewData["Count"] = expiredCount;
            return View("Index");
        }
    }
}
