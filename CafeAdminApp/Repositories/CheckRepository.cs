using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    /// <summary>
    /// Repository for managing check-related database operations.
    /// </summary>
    public class CheckRepository : ICheckRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public CheckRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all checks from the database, including related orders.
        /// </summary>
        /// <returns>A list of all checks with their associated orders.</returns>
        public async Task<List<Check>> GetAllAsync()
        {
            return await _context.Checks.Include(c => c.Order).ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific check by its ID, including the related order.
        /// </summary>
        /// <param name="id">The ID of the check to retrieve.</param>
        /// <returns>The check with the specified ID, or null if not found.</returns>
        public async Task<Check?> GetByIdAsync(int id)
        {
            return await _context.Checks.Include(c => c.Order).FirstOrDefaultAsync(c => c.CheckId == id);
        }

        /// <summary>
        /// Adds a new check to the database.
        /// </summary>
        /// <param name="check">The check to add.</param>
        public async Task AddAsync(Check check)
        {
            await _context.Checks.AddAsync(check);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing check in the database.
        /// </summary>
        /// <param name="check">The check to update.</param>
        public async Task UpdateAsync(Check check)
        {
            _context.Checks.Update(check);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a check from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the check to delete.</param>
        public async Task DeleteAsync(int id)
        {
            var check = await _context.Checks.FindAsync(id);
            if (check != null)
            {
                _context.Checks.Remove(check);
                await _context.SaveChangesAsync();
            }
        }
    }
}
