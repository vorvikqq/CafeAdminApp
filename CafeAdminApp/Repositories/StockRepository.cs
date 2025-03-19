using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CafeAdminApp.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockItem>> GetAllAsync()
        {
            return await _context.Stock.Include(s => s.Product).ToListAsync();
        }

        public async Task<StockItem?> GetByIdAsync(int id)
        {
            return await _context.Stock.Include(s => s.Product).FirstOrDefaultAsync(s => s.StockId == id);
        }

        public async Task AddAsync(StockItem stockItem)
        {
            _context.Stock.Add(stockItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockItem stockItem)
        {
            _context.Stock.Update(stockItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var stockItem = await _context.Stock.FindAsync(id);
            if (stockItem != null)
            {
                _context.Stock.Remove(stockItem);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Змінити isProsporchka на true для продуктів, у яких закінчився срок споживання та вони ще не були позначені як просрочені
        /// </summary>
        /// <param name="expiredProductIds"> Айді продуктів, у яких закінчився срок споживання</param>
        /// <returns> Кількість предметів Стоку, у яких було змінено isProsrochka = true</returns>
        public async Task<int> SetExpiredProductsAsync(List<int> expiredProductIds)
        {
            var expiredStockItems = await _context.Stock.
                Where(s => expiredProductIds.Contains(s.ProductId) && !s.IsProsrochka).ToListAsync();

            if (!expiredStockItems.Any()) 
                return 0;

            expiredStockItems.ForEach(s => s.IsProsrochka = true);
            await _context.SaveChangesAsync();

            return expiredStockItems.Count;
        }

        /// <summary>
        /// Додати продукти до Стоку за айді цін
        /// </summary>
        /// <param name="priceIds"> айді цін</param>
        /// <returns></returns>
        public async Task AddProductsByIds(List<int> priceIds)
        {
            var productsWithQuantities = await _context.InvoicePrice
            .Where(ip => priceIds.Contains(ip.PriceId)) 
            .Join(_context.Prices, // Join InvoicePrice i Prices
                  ip => ip.PriceId, // PriceId в InvoicePrice
                  p => p.PriceId,   // PriceId в Prices
                  (ip, p) => new   // Створюємо новий об'єкт (результат джойну)
                  {
                      ProductId = p.ProductId, // ProductId із таблиці Prices 
                      Quantity = ip.Quantity  // Quantity із таблиці InvoicePrice
                  })
            .ToListAsync();

            foreach (var product in productsWithQuantities)
            {
                var stockItem = await _context.Stock.FirstOrDefaultAsync(s => s.ProductId == product.ProductId);

                // Перевірка щоб не можна було додати з одного інвойсу один продукт багато разів.
                if (stockItem == null)
                {
                    _context.Stock.Add(new StockItem
                    {
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        IsProsrochka = false
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
