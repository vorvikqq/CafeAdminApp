using CafeAdminApp.Models.ViewModels;
using CafeAdminApp.Repositories.Interfaces;
using ClosedXML.Excel;
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
            ViewData["ActivePage"] = "Report";

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
            model.NetProfitDetails = await _reportRepository.GetNetProfitReport(model.StartDate, model.EndDate);
            model.TopSellingProducts = await _reportRepository.GetTopSellingProducts(model.StartDate, model.EndDate);
            model.StockAvailability = await _reportRepository.GetStockReport();
            model.SpoiledGoods = await _reportRepository.GetSpoiledProductsReport();

            return View("Index", model);
        }


        [HttpGet]
        public async Task<IActionResult> ExportToExcel(DateTime startDate, DateTime endDate)
        {
            //var startDate = DateTime.Today.AddDays(-30); // Наприклад, останній місяць
            //var endDate = DateTime.Today;

            var netProfit = await _reportRepository.GetNetProfitReport(startDate, endDate);
            var topSellingProducts = await _reportRepository.GetTopSellingProducts(startDate, endDate);
            var stockAvailability = await _reportRepository.GetStockReport();
            var spoiledGoods = await _reportRepository.GetSpoiledProductsReport();

            using var workbook = new XLWorkbook();

            // Лист 1: Прибуток
            var profitSheet = workbook.Worksheets.Add("Прибуток");

            // Заголовки таблиці
            profitSheet.Cell(1, 1).Value = "№";
            profitSheet.Cell(1, 2).Value = "Назва";
            profitSheet.Cell(1, 3).Value = "Кількість продано";
            profitSheet.Cell(1, 4).Value = "Ціна закупівлі";
            profitSheet.Cell(1, 5).Value = "Ціна продажу";
            profitSheet.Cell(1, 6).Value = "Прибуток";
            profitSheet.Row(1).Style.Font.Bold = true;

            // Заповнення даних
            int row = 2;
            foreach (var item in netProfit)
            {
                profitSheet.Cell(row, 1).Value = row - 1; // №
                profitSheet.Cell(row, 2).Value = item.ProductName; // Назва
                profitSheet.Cell(row, 3).Value = item.QuantitySold; // Кількість продано
                profitSheet.Cell(row, 4).Value = item.BoughtPrice; // Ціна закупівлі
                profitSheet.Cell(row, 5).Value = item.SellPrice; // Ціна продажу
                profitSheet.Cell(row, 6).Value = item.TotalProfit; // Прибуток
                row++;
            }

            // Загальний чистий прибуток
            profitSheet.Cell(row, 4).Value = "Загальний чистий прибуток:";
            profitSheet.Range(row, 4, row, 5).Merge();
            profitSheet.Cell(row, 6).Value = netProfit.Sum(x => x.TotalProfit) + " грн";
            profitSheet.Row(row).Style.Font.Bold = true;

            // Автоматичне налаштування ширини колонок
            profitSheet.Columns().AdjustToContents();

            // Лист 2: Топ продажі
            var topSheet = workbook.Worksheets.Add("Топ продажі");
            topSheet.Cell(1, 1).Value = "Назва товару";
            topSheet.Cell(1, 2).Value = "Категорія";
            topSheet.Cell(1, 3).Value = "Продано";
            topSheet.Row(1).Style.Font.Bold = true;

            row = 2;
            foreach (var item in topSellingProducts)
            {
                topSheet.Cell(row, 1).Value = item.ProductName;
                topSheet.Cell(row, 2).Value = item.CategoryName;
                topSheet.Cell(row, 3).Value = item.TotalSold;
                row++;
            }

            // Лист 3: Наявність товарів
            var stockSheet = workbook.Worksheets.Add("Наявність товарів");
            stockSheet.Cell(1, 1).Value = "Назва товару";
            stockSheet.Cell(1, 2).Value = "Кількість";
            stockSheet.Row(1).Style.Font.Bold = true;

            row = 2;
            foreach (var item in stockAvailability)
            {
                stockSheet.Cell(row, 1).Value = item.ProductName;
                stockSheet.Cell(row, 2).Value = item.Quantity;
                row++;
            }

            // Лист 4: Зіпсовані товари
            var spoiledSheet = workbook.Worksheets.Add("Зіпсовані товари");
            spoiledSheet.Cell(1, 1).Value = "Назва товару";
            spoiledSheet.Cell(1, 2).Value = "Кількість списаних";
            spoiledSheet.Cell(1, 3).Value = "Сума втрат (грн)";
            spoiledSheet.Row(1).Style.Font.Bold = true;

            row = 2;
            foreach (var item in spoiledGoods)
            {
                spoiledSheet.Cell(row, 1).Value = item.ProductName;
                spoiledSheet.Cell(row, 2).Value = item.TotalQuantity;
                spoiledSheet.Cell(row, 3).Value = item.TotalCost;
                row++;
            }

            // Загальна втрата
            spoiledSheet.Cell(row, 1).Value = "Загальна втрата:";
            spoiledSheet.Range(row, 1, row, 2).Merge();
            spoiledSheet.Cell(row, 3).Value = spoiledGoods.Sum(x => x.TotalCost) + " грн";
            spoiledSheet.Row(row).Style.Font.Bold = true;

            // Автоматичне налаштування ширини колонок
            foreach (var sheet in workbook.Worksheets)
            {
                sheet.Columns().AdjustToContents();
            }

            // Генеруємо назву файлу з датами
            var fileName = $"Звіт {startDate.ToString("dd.MM.yyyy")} - {endDate.ToString("dd.MM.yyyy")}.xlsx";

            // Повертаємо файл Excel з динамічною назвою
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

    }
}
