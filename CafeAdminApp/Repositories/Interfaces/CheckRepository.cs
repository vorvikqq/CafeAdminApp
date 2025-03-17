using CafeAdminApp.Models;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface ICheckRepository 
    {
        Task<List<Check>> GetAllAsync();
        Task<Check?> GetByIdAsync(int id);
        Task AddAsync(Check check);
        Task UpdateAsync(Check check);
        Task DeleteAsync(int id);
    }
}
