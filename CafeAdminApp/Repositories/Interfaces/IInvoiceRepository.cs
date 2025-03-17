using CafeAdminApp.Models;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<List<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(int id);
    }
}
