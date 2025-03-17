using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    public class PriceRepository : IPriceRepository
    {
        private readonly ApplicationDbContext _context;

        public PriceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Price>> GetAllAsync()
        {
            return await _context.Prices
                .Include(p => p.Product)
                .Include(p => p.InvoicePrices)
                .Include(p => p.OrderPrices)
                .ToListAsync();
        }

        public async Task<Price?> GetByIdAsync(int id)
        {
            return await _context.Prices.Include(p => p.Product)
                .Include(p => p.InvoicePrices)
                .Include(p => p.OrderPrices)
                .FirstOrDefaultAsync(p => p.PriceId == id);
        }
    }
}
