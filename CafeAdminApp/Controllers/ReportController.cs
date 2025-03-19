using CafeAdminApp.Models.ViewModels;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CafeAdminApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;

        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public IActionResult Index()
        {
            var model = new ReportFilterViewModel
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                Filter = "day"
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReport(ReportFilterViewModel model)
        {
            // Визначаємо діапазон дат відповідно до вибраного фільтра
            model.SetDateRange();

            // Отримання даних звітів
            model.NetProfit = await _reportRepository.GetNetProfitReport(model.StartDate, model.EndDate);
            model.TopSellingProducts = await _reportRepository.GetTopSellingProducts(model.StartDate, model.EndDate);
            model.StockAvailability = await _reportRepository.GetStockReport();
            model.SpoiledGoods = await _reportRepository.GetSpoiledProductsReport();

            return View("Index", model);
        }
    }
}
