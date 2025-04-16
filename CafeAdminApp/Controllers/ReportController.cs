using CafeAdminApp.Models.ViewModels;
using CafeAdminApp.Repositories.Interfaces;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;


namespace CafeAdminApp.Controllers
{
    /// <summary>
    /// Controller responsible for generating and exporting reports.
    /// </summary>
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportController"/> class.
        /// </summary>
        /// <param name="reportRepository">The report repository used to fetch report data.</param>
        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        /// <summary>
        /// Displays the initial report view with default filters.
        /// </summary>
        /// <returns>The report view with default filter model.</returns>
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

        /// <summary>
        /// Generates a report based on selected filters.
        /// </summary>
        /// <param name="model">The filter model including date range and filter type.</param>
        /// <returns>The updated report view with data populated from the repository.</returns>
        [HttpPost]
        public async Task<IActionResult> GenerateReport(ReportFilterViewModel model)
        {
            // Determine the date range based on the selected filter
            model.SetDateRange();

            // Fetch report data from the repository
            model.NetProfitDetails = await _reportRepository.GetNetProfitReport(model.StartDate, model.EndDate);
            model.TopSellingProducts = await _reportRepository.GetTopSellingProducts(model.StartDate, model.EndDate);
            model.StockAvailability = await _reportRepository.GetStockReport();
            model.SpoiledGoods = await _reportRepository.GetSpoiledProductsReport();

            return View("Index", model);
        }

        /// <summary>
        /// Exports the report data to an Excel file for the given date range.
        /// </summary>
        /// <param name="startDate">Start date of the report period.</param>
        /// <param name="endDate">End date of the report period.</param>
        /// <returns>An Excel file containing the generated report.</returns>
        [HttpGet]
        public async Task<IActionResult> ExportToExcel(DateTime startDate, DateTime endDate)
        {
            var netProfit = await _reportRepository.GetNetProfitReport(startDate, endDate);
            var topSellingProducts = await _reportRepository.GetTopSellingProducts(startDate, endDate);
            var stockAvailability = await _reportRepository.GetStockReport();
            var spoiledGoods = await _reportRepository.GetSpoiledProductsReport();

            using var workbook = new XLWorkbook();

            // Sheet 1: Net Profit
            var profitSheet = workbook.Worksheets.Add("Прибуток");
            profitSheet.Cell(1, 1).Value = "№";
            profitSheet.Cell(1, 2).Value = "Назва";
            profitSheet.Cell(1, 3).Value = "Кількість продано";
            profitSheet.Cell(1, 4).Value = "Ціна закупівлі";
            profitSheet.Cell(1, 5).Value = "Ціна продажу";
            profitSheet.Cell(1, 6).Value = "Прибуток";
            profitSheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            foreach (var item in netProfit)
            {
                profitSheet.Cell(row, 1).Value = row - 1;
                profitSheet.Cell(row, 2).Value = item.ProductName;
                profitSheet.Cell(row, 3).Value = item.QuantitySold;
                profitSheet.Cell(row, 4).Value = item.BoughtPrice;
                profitSheet.Cell(row, 5).Value = item.SellPrice;
                profitSheet.Cell(row, 6).Value = item.TotalProfit;
                row++;
            }

            profitSheet.Cell(row, 4).Value = "Загальний чистий прибуток:";
            profitSheet.Range(row, 4, row, 5).Merge();
            profitSheet.Cell(row, 6).Value = netProfit.Sum(x => x.TotalProfit) + " грн";
            profitSheet.Row(row).Style.Font.Bold = true;
            profitSheet.Columns().AdjustToContents();

            // Sheet 2: Top Selling Products
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

            // Sheet 3: Stock Availability
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

            // Sheet 4: Spoiled Goods
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

            spoiledSheet.Cell(row, 1).Value = "Загальна втрата:";
            spoiledSheet.Range(row, 1, row, 2).Merge();
            spoiledSheet.Cell(row, 3).Value = spoiledGoods.Sum(x => x.TotalCost) + " грн";
            spoiledSheet.Row(row).Style.Font.Bold = true;

            foreach (var sheet in workbook.Worksheets)
            {
                sheet.Columns().AdjustToContents();
            }

            var fileName = $"Звіт {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}.xlsx";

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
