using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    /// <summary>
    /// Repository for managing invoice-related database operations.
    /// </summary>
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all invoices from the database, including their associated invoice prices.
        /// </summary>
        /// <returns>A list of invoices with their related invoice prices.</returns>
        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.Include(i => i.InvoicePrices).ToListAsync();
        }

        /// <summary>
        /// Retrieves all invoices from the database without including the related InvoicePrices table.
        /// </summary>
        /// <returns>A list of invoices without related invoice prices.</returns>
        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific invoice by its ID, including related invoice prices.
        /// </summary>
        /// <param name="id">The ID of the invoice.</param>
        /// <returns>The invoice with its prices, or null if not found.</returns>
        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices.Include(i => i.InvoicePrices).FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        /// <summary>
        /// Retrieves all price IDs associated with a specific invoice.
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice.</param>
        /// <returns>A list of price IDs linked to the given invoice.</returns>
        public async Task<List<int>> GetAllPricesForInvoiceAsync(int invoiceId)
        {
            return await _context.InvoicePrice
                .Where(ip => ip.InvoiceId == invoiceId)
                .Select(ip => ip.PriceId)
                .ToListAsync();
        }

        /// <summary>
        /// Deletes a specific invoice by its ID.
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice to delete.</param>
        public async Task DeleteAsync(int invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes all invoice prices associated with a specific invoice.
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice whose prices should be deleted.</param>
        public async Task DeleteInvoicePricesAsync(int invoiceId)
        {
            var invoicePricesToDelete = await _context.InvoicePrice
                .Where(ip => ip.InvoiceId == invoiceId)
                .ToListAsync();
            if (invoicePricesToDelete.Count > 0)
            {
                _context.InvoicePrice.RemoveRange(invoicePricesToDelete);
            }
            await _context.SaveChangesAsync();
        }
    }
}
