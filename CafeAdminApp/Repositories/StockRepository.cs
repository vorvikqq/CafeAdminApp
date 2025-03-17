using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockItem>> GetAllAsync()
        {
            return await _context.Stock.Include(s => s.Product).ToListAsync();
        }

        public async Task<StockItem?> GetByIdAsync(int id)
        {
            return await _context.Stock.Include(s => s.Product).FirstOrDefaultAsync(s => s.StockId == id);
        }

        public async Task AddAsync(StockItem stockItem)
        {
            _context.Stock.Add(stockItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockItem stockItem)
        {
            _context.Stock.Update(stockItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var stockItem = await _context.Stock.FindAsync(id);
            if (stockItem != null)
            {
                _context.Stock.Remove(stockItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
