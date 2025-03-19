using CafeAdminApp.Data;
using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.Include(i => i.InvoicePrices).ToListAsync();
        }

        /// <summary>
        /// отримати всі інвойси, БЕЗ таблиці InvoicePrices
        /// </summary>
        /// <returns></returns>
        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices.ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices.Include(i => i.InvoicePrices).FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        /// <summary>
        /// Отримати всі айді ціни для заданого інвойсу
        /// </summary>
        /// <param name="invoiceId"> айді інвойсу для якого шукаємо</param>
        /// <returns></returns>
        public async Task<List<int>> GetAllPricesForInvoiceAsync(int invoiceId)
        {
            return await _context.InvoicePrice
                .Where(ip => ip.InvoiceId == invoiceId)
                .Select(ip => ip.PriceId)
                .ToListAsync();
        }

        public async Task DeleteAsync(int invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

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
