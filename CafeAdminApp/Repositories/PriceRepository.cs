using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Models.ViewModels;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    /// <summary>
    /// Repository for managing price-related database operations.
    /// </summary>
    public class PriceRepository : IPriceRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public PriceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all prices from the database, including associated product details, invoice prices, and order prices.
        /// </summary>
        /// <returns>A list of prices with their related product and price details.</returns>
        public async Task<List<Price>> GetAllAsync()
        {
            return await _context.Prices
                .Include(p => p.Product)
                .Include(p => p.InvoicePrices)
                .Include(p => p.OrderPrices)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific price by its ID, including associated product details, invoice prices, and order prices.
        /// </summary>
        /// <param name="id">The ID of the price.</param>
        /// <returns>The price with its related product and price details, or null if not found.</returns>
        public async Task<Price?> GetByIdAsync(int id)
        {
            return await _context.Prices.Include(p => p.Product)
                .Include(p => p.InvoicePrices)
                .Include(p => p.OrderPrices)
                .FirstOrDefaultAsync(p => p.PriceId == id);
        }

        /// <summary>
        /// Retrieves the product details for the given price IDs, specifically for representing invoice details.
        /// </summary>
        /// <param name="priceIds">The list of price IDs to retrieve product details for.</param>
        /// <returns>A list of product details, including product name, price, and quantity.</returns>
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
        /// Retrieves the order details for the given price IDs and order ID.
        /// </summary>
        /// <param name="priceIds">The list of price IDs to retrieve order details for.</param>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>A list of order details, including product name, price, and quantity.</returns>
        public async Task<List<OrderDetails>> GetOrderDetailsAsync(List<int> priceIds, int orderId)
        {
            return await _context.Prices
                .Where(p => priceIds.Contains(p.PriceId))
                .Select(p => new OrderDetails
                {
                    ProductName = p.Product.ProductName,
                    Price = p.SellPrice,
                    Quantity = p.OrderPrices.FirstOrDefault(op => op.OrderId == orderId && priceIds.Contains(op.PriceId)).Quantity
                })
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves the product IDs associated with the given price IDs.
        /// </summary>
        /// <param name="priceIds">The list of price IDs to retrieve product IDs for.</param>
        /// <returns>A list of product IDs associated with the given price IDs.</returns>
        public async Task<List<int>> GetProductIdsByPriceIds(List<int> priceIds)
        {
            return await _context.Prices
                                 .Where(p => priceIds.Contains(p.PriceId))
                                 .Select(p => p.ProductId)
                                 .ToListAsync();
        }

        /// <summary>
        /// Deletes multiple prices by their IDs.
        /// </summary>
        /// <param name="priceIds">The list of price IDs to delete.</param>
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
