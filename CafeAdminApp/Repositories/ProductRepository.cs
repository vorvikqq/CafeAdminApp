using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    /// <summary>
    /// Repository for managing product-related database operations.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all products from the database, including their associated category.
        /// </summary>
        /// <returns>A list of products with their associated category.</returns>
        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific product by its ID, including its associated category.
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The product with its associated category, or null if not found.</returns>
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductId == id);
        }

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The product to be added.</param>
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="id">The ID of the product to be updated.</param>
        /// <param name="product">The updated product data.</param>
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

        /// <summary>
        /// Deletes a product from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to be deleted.</param>
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Checks if a product exists in the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to check.</param>
        /// <returns>True if the product exists, otherwise false.</returns>
        public bool IsExist(int id)
        {
            return _context.Products.Any(p => p.ProductId == id);
        }

        /// <summary>
        /// Retrieves a list of products that have expired based on their consumption date.
        /// </summary>
        /// <returns>A list of expired products.</returns>
        public async Task<List<Product>> GetExpiredProductsAsync()
        {
            return await _context.Products.Where(p => p.ConsumptionDate < DateTimeOffset.UtcNow).ToListAsync();
        }

        /// <summary>
        /// Deletes multiple products by their IDs.
        /// </summary>
        /// <param name="productsIdsToDelete">The list of product IDs to be deleted.</param>
        public async Task DeleteManyByIdsAsync(List<int> productsIdsToDelete)
        {
            var productsToDelete = await _context.Products
                .Where(p => productsIdsToDelete.Contains(p.ProductId))
                .ToListAsync();

            _context.Products.RemoveRange(productsToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
