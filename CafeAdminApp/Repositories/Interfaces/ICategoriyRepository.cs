using CafeAdminApp.Models;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface ICategoriyRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int categoryId);
    }
}
