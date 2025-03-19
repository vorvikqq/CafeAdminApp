using CafeAdminApp.Models;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CafeAdminApp.Controllers
{
    public class InvoiceController : Controller
    {
        private IInvoiceRepository _invoiceRepository;
        private IPriceRepository _priceRepository;
        public InvoiceController(IInvoiceRepository invoiceRepository, IPriceRepository priceRepository)
        {
            _invoiceRepository = invoiceRepository;
            _priceRepository = priceRepository;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            return View("Index", invoices);
        }

        public async Task<IActionResult> InvoiceDetails(int invoiceId)
        {
            var priceIds = await _invoiceRepository.GetAllPricesForInvoiceAsync(invoiceId);

            if (priceIds == null || !priceIds.Any())
            {
                ViewData["Message"] = "Немає товарів у цьому інвойсі.";
                return View(new List<InvoiceProductDetails>());
            }

            // Отримуємо деталі продуктів (назва, ціна, кількість)
            var productDetails = await _priceRepository.GetProductDetailsAsync(priceIds);
            
            ViewData["InvoiceId"] = invoiceId;

            return View(productDetails);
        }

        [HttpPost]
        public IActionResult AcceptInvoice(int invoiceId)
        {
            // TODO: Додати до таблиці Stock усі продукти через айдішки для цього інвойсу
            //Замісти всі сліди: Видалити дані про продукти які були додані із таблиць Invoice, InvoicePrice.
            //Повернути у вью меседжБокс.
            // DeleteInvoices();
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult DiscardInvoice(int invoiceId)
        {
            // DeleteInvoices();
            // TODO:Замісти всі сліди: Видалити дані про продукти які були додані із таблиць Invoice, InvoicePrice.
            //Повернути у вью меседжБокс.
            throw new NotImplementedException();
        }

        private bool DeleteInvoices()
        {
            // TODO:
            // просто логіку видалення винести в окремий метод, бо використовується в двох випадках - прийняти і не прийняти інвойс.
            throw new NotImplementedException();
        }
    }
}
