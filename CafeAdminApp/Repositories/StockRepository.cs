using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all stock items from the database, including associated product details.
        /// </summary>
        /// <returns>A list of all stock items.</returns>
        public async Task<List<StockItem>> GetAllAsync()
        {
            return await _context.Stock.Include(s => s.Product).ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific stock item by its ID, including the associated product details.
        /// </summary>
        /// <param name="id">The ID of the stock item.</param>
        /// <returns>The stock item with the specified ID, or null if not found.</returns>
        public async Task<StockItem?> GetByIdAsync(int id)
        {
            return await _context.Stock.Include(s => s.Product).FirstOrDefaultAsync(s => s.StockId == id);
        }

        /// <summary>
        /// Adds a new stock item to the database.
        /// </summary>
        /// <param name="stockItem">The stock item to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddAsync(StockItem stockItem)
        {
            _context.Stock.Add(stockItem);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing stock item in the database.
        /// </summary>
        /// <param name="stockItem">The stock item to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(StockItem stockItem)
        {
            _context.Stock.Update(stockItem);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a stock item by its ID from the database.
        /// </summary>
        /// <param name="id">The ID of the stock item to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
        /// Marks products as expired by setting the IsProsrochka flag to true for products that have passed their expiration date and are not yet marked as expired.
        /// </summary>
        /// <param name="expiredProductIds">A list of product IDs whose expiration date has passed.</param>
        /// <returns>The number of stock items where IsProsrochka was set to true.</returns>
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
        /// Adds products to stock based on the provided price IDs.
        /// Ensures that a product from an invoice is not added multiple times.
        /// </summary>
        /// <param name="priceIds">A list of price IDs associated with the products to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddProductsByIds(List<int> priceIds)
        {
            var productsWithQuantities = await _context.InvoicePrice
                .Where(ip => priceIds.Contains(ip.PriceId))
                .Join(_context.Prices,
                      ip => ip.PriceId,
                      p => p.PriceId,
                      (ip, p) => new
                      {
                          ProductId = p.ProductId,
                          Quantity = ip.Quantity
                      })
                .ToListAsync();

            foreach (var product in productsWithQuantities)
            {
                var stockItem = await _context.Stock.FirstOrDefaultAsync(s => s.ProductId == product.ProductId);

                // Ensure that the same product is not added multiple times from one invoice
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

        /// <summary>
        /// Adds products to stock based on price IDs from a specific order.
        /// If a product already exists in stock, its quantity is updated.
        /// </summary>
        /// <param name="priceIds">A list of price IDs associated with the products to add.</param>
        /// <param name="orderId">The ID of the order containing the products.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddProductsByIdsFromOrder(List<int> priceIds, int orderId)
        {
            var productsWithQuantities = await _context.OrderPrice
                .Where(op => priceIds.Contains(op.PriceId) && op.OrderId == orderId)
                .Join(_context.Prices,
                      op => op.PriceId,
                      p => p.PriceId,
                      (op, p) => new
                      {
                          ProductId = p.ProductId,
                          Quantity = op.Quantity
                      })
                .ToListAsync();

            var productIds = productsWithQuantities.Select(p => p.ProductId).ToList();
            var existingStockItems = await _context.Stock
                .Where(s => productIds.Contains(s.ProductId))
                .ToListAsync();

            foreach (var product in productsWithQuantities)
            {
                var stockItem = existingStockItems.FirstOrDefault(s => s.ProductId == product.ProductId);

                if (stockItem == null)
                {
                    _context.Stock.Add(new StockItem
                    {
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        IsProsrochka = false
                    });
                }
                else
                {
                    stockItem.Quantity += product.Quantity;
                }
            }

            await _context.SaveChangesAsync();
        }
    }

}
