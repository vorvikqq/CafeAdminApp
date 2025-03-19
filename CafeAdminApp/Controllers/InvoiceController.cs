using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CafeAdminApp.Controllers
{
    public class InvoiceController : Controller
    {
        private IInvoiceRepository _invoiceRepository;
        private IPriceRepository _priceRepository;
        private IStockRepository _stockRepository;
        private IProductRepository _productRepository;
        public InvoiceController(IInvoiceRepository invoiceRepository, IPriceRepository priceRepository, IStockRepository stockRepository, IProductRepository productRepository)
        {
            _invoiceRepository = invoiceRepository;
            _priceRepository = priceRepository;
            _stockRepository = stockRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Отримати всі інвойси
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            return View("Index", invoices);
        }

        /// <summary>
        /// Отримати деталі інвойсу 
        /// </summary>
        /// <param name="invoiceId"> айді інвойсу деталі продуктів якого потрібно відобразити</param>
        /// <returns></returns>
        public async Task<IActionResult> InvoiceDetails(int invoiceId)
        {
            var priceIds = await _invoiceRepository.GetAllPricesForInvoiceAsync(invoiceId);

            ViewData["InvoiceId"] = invoiceId;

            if (priceIds == null || !priceIds.Any())
            {
                ViewData["Message"] = "Немає товарів у цьому інвойсі.";
                return View(new List<InvoiceProductDetails>());
            }

            // Отримуємо деталі продуктів (назва, ціна, кількість)
            var productDetails = await _priceRepository.GetInvoiceProductDetailsAsync(priceIds);

            return View(productDetails);
        }

        /// <summary>
        /// Додати інформацію про інвойс в Stock, видалити інформацію про інвойс
        /// </summary>
        /// <param name="invoiceId"> інвойс з яким працюємо</param>
        /// <returns></returns>
        public async Task<IActionResult> AcceptInvoice(int invoiceId)
        {
            var priceIds = await _invoiceRepository.GetAllPricesForInvoiceAsync(invoiceId);
            if (priceIds == null || !priceIds.Any())
            {
                ViewData["Message"] = "Помилка: В Таблиці Ціни немає ID продуктів з інвойсу";
                return View("InvoiceDetails", new List<InvoiceProductDetails>());
            }

            await _stockRepository.AddProductsByIds(priceIds);

            await CleanUpInvoices(invoiceId);

            ViewData["Message"] = "Інвойс успішно підтверджено та продукти додано в Stock.";
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            return View("Index", invoices);

        }

        /// <summary>
        /// НЕ додавати продукти в Stock, видалити продукти які були у інвойсів, видалити інформацію про інвойс
        /// </summary>
        /// <param name="invoiceId"> інвойс продукти якого видаляються </param>
        /// <returns></returns>
        public async Task<IActionResult> DiscardInvoice(int invoiceId)
        {
            var priceIds = await _invoiceRepository.GetAllPricesForInvoiceAsync(invoiceId);
            var productIds = await _priceRepository.GetProductIdsByPriceIds(priceIds);
            await CleanUpInvoices(invoiceId);
            await _priceRepository.DeleteManyByIdsAsync(priceIds);
            await _productRepository.DeleteManyByIdsAsync(productIds);

            ViewData["Message"] = "Інвойс успішно відхилено.";
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            return View("Index", invoices);
        }

        /// <summary>
        /// Видалення інформації про інвойс, інвойсПрайс
        /// </summary>
        /// <param name="invoiceId"></param>
        private async Task CleanUpInvoices(int invoiceId)
        {
            await _invoiceRepository.DeleteInvoicePricesAsync(invoiceId);
            await _invoiceRepository.DeleteAsync(invoiceId);
        }
    }
}
