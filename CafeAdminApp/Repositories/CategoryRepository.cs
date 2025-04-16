using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    /// <summary>
    /// Repository for managing category-related database operations.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all categories from the database.
        /// </summary>
        /// <returns>A list of all categories.</returns>
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific category by its ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to retrieve.</param>
        /// <returns>The category with the specified ID, or null if not found.</returns>
        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }
    }
}
