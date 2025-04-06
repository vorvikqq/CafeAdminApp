using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore;
using Order = CafeAdminApp.Models.Order;

namespace CafeAdminApp.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.Include(o => o.OrderPrices).ToListAsync();
        }

        public async Task<List<Order>> GetAllUnconfirmedAsync()
        {
            return await _context.Orders.Include(o => o.OrderPrices).Where(o => o.OrderStatus == false).ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(o => o.OrderPrices).FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<List<int>> GetAllPricesForOrderAsync(int orderId)
        {
            return await _context.OrderPrice.Where(op => op.OrderId == orderId).Select(op => op.PriceId).ToListAsync();
        }

        public async Task UpdateOrderStatus(int orderId, bool status)
        {
            await _context.Orders
                .Where(o => o.OrderId == orderId)
                .ExecuteUpdateAsync(o => o.SetProperty(x => x.OrderStatus, status));
        }

        public async Task DeleteOrderPricesAsync(int orderId)
        {
           await _context.OrderPrice
                .Where(op => op.OrderId == orderId)
                .ExecuteDeleteAsync();
        }

        public async Task DeleteAsync(int orderId)
        {
           await _context.Orders
                .Where(o => o.OrderId == orderId)
                .ExecuteDeleteAsync();
        }
    }
}
