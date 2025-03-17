using CafeAdminApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Controllers
{
    public class ProductExpirationController : Controller
    {
        private readonly IStockRepository _stockRepo;
        private readonly IProductRepository _prodRepo;
        public ProductExpirationController(IStockRepository stockRepository, IProductRepository productRepository)
        {
            _stockRepo = stockRepository;
            _prodRepo = productRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Списання продуктів: Змінити isProsporchka на true для продуктів, які просрочені    
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetExpirationFlag()
        {
            var expiredProducts = await _prodRepo.GetExpiredProductsAsync();

            if (!expiredProducts.Any())
            {
                ViewData["Message"] = "Немає просрочених продуктів.";
                ViewData["Count"] = 0;
                return View("Index");
            }

            var expiredProductIds = expiredProducts.Select(p => p.ProductId).ToList();

            var expiredCount = await _stockRepo.SetExpiredProductsAsync(expiredProductIds);

            ViewData["Message"] = $"Списано {expiredCount} продуктів.";
            ViewData["Count"] = expiredCount;
            return View("Index");
        }
    }
}
