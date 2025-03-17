using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    public class CheckRepository : ICheckRepository
    {
        private readonly ApplicationDbContext _context;

        public CheckRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Check>> GetAllAsync()
        {
            return await _context.Checks.Include(c => c.Order).ToListAsync();
        }

        public async Task<Check?> GetByIdAsync(int id)
        {
            return await _context.Checks.Include(c => c.Order).FirstOrDefaultAsync(c => c.CheckId == id);
        }

        public async Task AddAsync(Check check)
        {
            await _context.Checks.AddAsync(check);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Check check)
        {
            _context.Checks.Update(check);
            await _context.SaveChangesAsync();
        }

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
