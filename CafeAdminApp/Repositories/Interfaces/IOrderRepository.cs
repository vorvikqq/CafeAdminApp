using CafeAdminApp.Models;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetAllUnconfirmedAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<List<int>> GetAllPricesForOrderAsync(int orderId);
    }
}
