using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);
        }
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (existingProduct is not null)
            {
                existingProduct.ProductName = product.ProductName;
                existingProduct.ManufactureDate = product.ManufactureDate;
                existingProduct.ConsumptionDate = product.ConsumptionDate;
                existingProduct.CategoryId = product.CategoryId;
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public bool IsExist(int id)
        {
            return _context.Products.Any(p => p.ProductId == id);
        }

        /// <summary>
        /// Отримати список продуктів, у яких закінчився термін придатності
        /// </summary>
        /// <returns> список Product, у яких закінчився термін придатності </returns>
        public async Task<List<Product>> GetExpiredProductsAsync()
        {
            return await _context.Products.Where(p => p.ConsumptionDate < DateTimeOffset.UtcNow).ToListAsync();
        }

    }
}
