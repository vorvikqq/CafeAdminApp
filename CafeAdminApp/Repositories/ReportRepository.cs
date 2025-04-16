using CafeAdminApp.Data;
using CafeAdminApp.Models.DtoModel;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    /// <summary>
    /// Repository for generating various reports related to sales, stock, and profits.
    /// </summary>
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Generates a report for net profit between the given start and end dates.
        /// </summary>
        /// <param name="startDate">The start date for the report period.</param>
        /// <param name="endDate">The end date for the report period.</param>
        /// <returns>A list of <see cref="NetProfitDto"/> containing profit details for the specified period.</returns>
        public async Task<List<NetProfitDto>> GetNetProfitReport(DateTime startDate, DateTime endDate)
        {
            var profitDetails = await _context.Orders
                .Where(o => o.OrderStatus == true && o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Join(_context.OrderPrice, o => o.OrderId, op => op.OrderId, (o, op) => new { o, op })
                .Join(_context.Prices, temp => temp.op.PriceId, p => p.PriceId, (temp, p) => new NetProfitDto
                {
                    ProductName = p.Product.ProductName,
                    QuantitySold = temp.op.Quantity,
                    BoughtPrice = (decimal)p.BoughtPrice,
                    SellPrice = (decimal)p.SellPrice,
                    TotalProfit = (decimal)(temp.op.Quantity * (p.SellPrice - p.BoughtPrice))
                })
                .ToListAsync();

            return profitDetails;
        }

        /// <summary>
        /// Generates a report for the top-selling products between the given start and end dates.
        /// </summary>
        /// <param name="startDate">The start date for the report period.</param>
        /// <param name="endDate">The end date for the report period.</param>
        /// <returns>A list of <see cref="TopSellingProductDto"/> containing top-selling products for the specified period.</returns>
        public async Task<List<TopSellingProductDto>> GetTopSellingProducts(DateTime startDate, DateTime endDate)
        {
            var products = await _context.Orders
                .Where(o => o.OrderStatus == true && o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Join(_context.OrderPrice, o => o.OrderId, op => op.OrderId, (o, op) => new { o, op })
                .Join(_context.Prices, temp => temp.op.PriceId, p => p.PriceId, (temp, p) => new { temp, p })
                .Join(_context.Products, temp => temp.p.ProductId, prod => prod.ProductId, (temp, prod) => new { temp, prod })
                .Join(_context.Categories, temp => temp.prod.CategoryId, c => c.CategoryId, (temp, c) => new
                {
                    temp.prod.ProductName,
                    CategoryName = c.CategoryName,
                    TotalSold = temp.temp.temp.op.Quantity
                })
                .GroupBy(p => new { p.ProductName, p.CategoryName })
                .Select(g => new TopSellingProductDto
                {
                    ProductName = g.Key.ProductName,
                    CategoryName = g.Key.CategoryName,
                    TotalSold = g.Sum(x => x.TotalSold)
                })
                .OrderByDescending(p => p.TotalSold)
                .ToListAsync();

            return products;
        }

        /// <summary>
        /// Generates a report for the current stock availability.
        /// </summary>
        /// <returns>A list of <see cref="StockItemDto"/> containing stock items and their quantities.</returns>
        public async Task<List<StockItemDto>> GetStockReport()
        {
            var stock = await _context.Stock
                .Join(_context.Products, s => s.ProductId, p => p.ProductId, (s, p) => new { s, p })
                .Join(_context.Categories, temp => temp.p.CategoryId, c => c.CategoryId, (temp, c) => new StockItemDto
                {
                    ProductName = temp.p.ProductName,
                    CategoryName = c.CategoryName,
                    Quantity = temp.s.Quantity,
                    IsSpoiled = temp.s.IsProsrochka
                })
                .OrderBy(p => p.CategoryName)
                .ThenBy(p => p.ProductName)
                .ToListAsync();

            return stock;
        }

        /// <summary>
        /// Generates a report for spoiled goods (expired items).
        /// </summary>
        /// <returns>A list of <see cref="SpoiledGoodsDto"/> containing information about spoiled goods.</returns>
        public async Task<List<SpoiledGoodsDto>> GetSpoiledProductsReport()
        {
            var spoiledProducts = await (
                from s in _context.Stock
                where s.IsProsrochka
                join pr in _context.Prices on s.ProductId equals pr.ProductId into priceGroup
                from pr in priceGroup.OrderByDescending(p => p.Date).Take(1).DefaultIfEmpty()
                join p in _context.Products on s.ProductId equals p.ProductId
                group new { s, pr } by new { s.ProductId, p.ProductName } into g
                select new SpoiledGoodsDto
                {
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(x => x.s.Quantity),
                    TotalCost = (decimal)g.Sum(x => x.s.Quantity * (x.pr != null ? x.pr.BoughtPrice : 0))
                }
            ).OrderByDescending(sp => sp.TotalCost)
            .ToListAsync();

            return spoiledProducts;
        }
    }
}
