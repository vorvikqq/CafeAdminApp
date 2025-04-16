using CafeAdminApp.Data;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Order = CafeAdminApp.Models.Order;

namespace CafeAdminApp.Repositories
{
    /// <summary>
    /// Repository for managing order-related database operations.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all orders from the database, including their associated order prices.
        /// </summary>
        /// <returns>A list of orders with their related order prices.</returns>
        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.Include(o => o.OrderPrices).ToListAsync();
        }

        /// <summary>
        /// Retrieves all unconfirmed orders from the database, including their associated order prices.
        /// </summary>
        /// <returns>A list of unconfirmed orders with their related order prices.</returns>
        public async Task<List<Order>> GetAllUnconfirmedAsync()
        {
            return await _context.Orders.Include(o => o.OrderPrices).Where(o => o.OrderStatus == false).ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific order by its ID, including related order prices.
        /// </summary>
        /// <param name="id">The ID of the order.</param>
        /// <returns>The order with its prices, or null if not found.</returns>
        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.OrderPrices).FirstOrDefaultAsync(o => o.OrderId == id);
        }

        /// <summary>
        /// Retrieves all price IDs associated with a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <returns>A list of price IDs linked to the given order.</returns>
        public async Task<List<int>> GetAllPricesForOrderAsync(int orderId)
        {
            return await _context.OrderPrice.Where(op => op.OrderId == orderId).Select(op => op.PriceId).ToListAsync();
        }

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">The ID of the order.</param>
        /// <param name="status">The new status of the order.</param>
        public async Task UpdateOrderStatus(int orderId, bool status)
        {
            await _context.Orders
                .Where(o => o.OrderId == orderId)
                .ExecuteUpdateAsync(o => o.SetProperty(x => x.OrderStatus, status));
        }

        /// <summary>
        /// Deletes all prices associated with a specific order.
        /// </summary>
        /// <param name="orderId">The ID of the order whose prices should be deleted.</param>
        public async Task DeleteOrderPricesAsync(int orderId)
        {
            await _context.OrderPrice
                .Where(op => op.OrderId == orderId)
                .ExecuteDeleteAsync();
        }

        /// <summary>
        /// Deletes a specific order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to delete.</param>
        public async Task DeleteAsync(int orderId)
        {
            await _context.Orders
                .Where(o => o.OrderId == orderId)
                .ExecuteDeleteAsync();
        }
    }
}
