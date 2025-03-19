using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Models.DtoModel;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Звіт по прибутку
        public async Task<decimal> GetNetProfitReport(DateTime startDate, DateTime endDate)
        {
            var netProfit = await _context.Orders
                .Where(o => o.OrderStatus == true && o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Join(_context.OrderPrice, o => o.OrderId, op => op.OrderId, (o, op) => new { o, op })
                .Join(_context.Prices, temp => temp.op.PriceId, p => p.PriceId, (temp, p) =>
                    temp.op.Quantity * (p.SellPrice - p.BoughtPrice))
                .SumAsync();

            return (decimal)netProfit;
        }

        // Звіт про найбільш продавані товари
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

        // Звіт про наявність товарів на складі
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


        // Звіт про баланс зіпсованих товарів
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
