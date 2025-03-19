﻿using CafeAdminApp.Data;
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

        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices.ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices.Include(i => i.InvoicePrices).FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public async Task<List<int>> GetAllPricesForInvoiceAsync(int invoiceId)
        {
            return await _context.InvoicePrice
                .Where(ip => ip.InvoiceId == invoiceId)
                .Select(ip => ip.PriceId)
                .ToListAsync();
        }

    }
}
