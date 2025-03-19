using CafeAdminApp.Models;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface IPriceRepository
    {
        Task<List<Price>> GetAllAsync();
        Task<Price?> GetByIdAsync(int id);
        Task<List<InvoiceProductDetails>> GetInvoiceProductDetailsAsync(List<int> priceIds);
        Task<List<int>> GetProductIdsByPriceIds(List<int> priceIds);
        Task DeleteManyByIdsAsync(List<int> priceIds);
    }
}
