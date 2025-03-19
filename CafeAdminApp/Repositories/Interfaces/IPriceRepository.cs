using CafeAdminApp.Models;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface IPriceRepository
    {
        Task<List<Price>> GetAllAsync();
        Task<Price?> GetByIdAsync(int id);
        Task<List<InvoiceProductDetails>> GetProductDetailsAsync(List<int> priceIds);
    }
}
