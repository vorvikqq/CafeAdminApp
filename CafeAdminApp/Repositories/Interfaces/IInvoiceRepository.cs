using CafeAdminApp.Models;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<List<Invoice>> GetAllAsync();
        Task<List<Invoice>> GetAllInvoicesAsync();
        Task<Invoice?> GetByIdAsync(int id);
        Task<List<int>> GetAllPricesForInvoiceAsync(int invoiceId);
    }
}
