using CafeAdminApp.Models;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface IStockRepository
    {
        Task<List<StockItem>> GetAllAsync();
        Task<StockItem?> GetByIdAsync(int id);
        Task AddAsync(StockItem stockItem);
        Task UpdateAsync(StockItem stockItem);
        Task DeleteAsync(int id);
    }
}
