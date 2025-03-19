using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    public class PriceRepository : IPriceRepository
    {
        private readonly ApplicationDbContext _context;

        public PriceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Price>> GetAllAsync()
        {
            return await _context.Prices
                .Include(p => p.Product)
                .Include(p => p.InvoicePrices)
                .Include(p => p.OrderPrices)
                .ToListAsync();
        }

        public async Task<Price?> GetByIdAsync(int id)
        {
            return await _context.Prices.Include(p => p.Product)
                .Include(p => p.InvoicePrices)
                .Include(p => p.OrderPrices)
                .FirstOrDefaultAsync(p => p.PriceId == id);
        }

        /// <summary>
        /// Отримати всі деталі продуктів для заданих цін (лише ті деталі, які потрібні для представлення інвойсу)
        /// </summary>
        /// <param name="priceIds"> айді цін з яких беремо дані</param>
        /// <returns> Список із деталей, які потрібні для представлення інвойсу </returns>
        public async Task<List<InvoiceProductDetails>> GetInvoiceProductDetailsAsync(List<int> priceIds)
        {
            return await _context.Prices
                .Where(p => priceIds.Contains(p.PriceId))
                .Select(p => new InvoiceProductDetails
                {
                    ProductName = p.Product.ProductName,
                    Price = p.BoughtPrice,
                    Quantity = p.InvoicePrices.FirstOrDefault(ip => priceIds.Contains(ip.PriceId)).Quantity
                })
                .ToListAsync();
        }

        /// <summary>
        /// Отримати айді продуктів по айді цін.
        /// </summary>
        /// <param name="priceIds"></param>
        /// <returns></returns>
        public async Task<List<int>> GetProductIdsByPriceIds(List<int> priceIds)
        {
            return await _context.Prices
                                 .Where(p => priceIds.Contains(p.PriceId))
                                 .Select(p => p.ProductId)
                                 .ToListAsync();
        }

        public async Task DeleteManyByIdsAsync(List<int> priceIds)
        {
            var pricesToDelete = await _context.Prices
                .Where(p => priceIds.Contains(p.PriceId))
                .ToListAsync();

            _context.Prices.RemoveRange(pricesToDelete);
            await _context.SaveChangesAsync();
        }


    }
}
