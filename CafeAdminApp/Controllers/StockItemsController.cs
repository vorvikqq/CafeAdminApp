using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;

namespace CafeAdminApp.Controllers
{
    public class StockItemsController : Controller
    {
        private readonly IStockRepository _stockRepo;

        public StockItemsController(IStockRepository stockRepository)
        {
            _stockRepo = stockRepository;
        }

        // GET: StockItems
        public async Task<IActionResult> Index()
        {
            return View(await _stockRepo.GetAllAsync());
        }

        // GET: StockItems/Details/5
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
