using CafeAdminApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CafeAdminApp.Controllers
{
    /// <summary>
    /// Controller for managing stock items in the inventory.
    /// </summary>
    public class StockItemsController : Controller
    {
        private readonly IStockRepository _stockRepo;

        /// <summary>
        /// Constructor for initializing the StockItemsController with a stock repository.
        /// </summary>
        /// <param name="stockRepository">Injected repository for accessing stock data.</param>
        public StockItemsController(IStockRepository stockRepository)
        {
            _stockRepo = stockRepository;
        }

        /// <summary>
        /// Displays a list of all stock items.
        /// </summary>
        /// <returns>A view with a list of all stock items.</returns>
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "StockItems";
            return View(await _stockRepo.GetAllAsync());
        }

        /// <summary>
        /// Displays the details of a specific stock item by ID.
        /// </summary>
        /// <param name="id">The ID of the stock item to display.</param>
        /// <returns>A view showing the stock item details, or NotFound if not found.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockItem = await _stockRepo.GetByIdAsync(id.Value);

            if (stockItem == null)
            {
                return NotFound();
            }

            return View(stockItem);
        }
    }

}
