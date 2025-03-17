using CafeAdminApp.Models;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(int id, Product product);
        Task DeleteAsync(int id);
        bool IsExist(int id);
        Task<List<Product>> GetExpiredProductsAsync();
    }
}
