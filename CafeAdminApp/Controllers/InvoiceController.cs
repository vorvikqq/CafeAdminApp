using CafeAdminApp.Models.ViewModels;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CafeAdminApp.Controllers
{
    public class InvoiceController : Controller
    {
        private IInvoiceRepository _invoiceRepository;
        private IPriceRepository _priceRepository;
        private IStockRepository _stockRepository;
        private IProductRepository _productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceController"/> class.
        /// </summary>
        /// <param name="invoiceRepository">Repository for handling invoice-related data.</param>
        /// <param name="priceRepository">Repository for handling price-related data.</param>
        /// <param name="stockRepository">Repository for managing stock data.</param>
        /// <param name="productRepository">Repository for managing product data.</param>
        public InvoiceController(IInvoiceRepository invoiceRepository, IPriceRepository priceRepository, IStockRepository stockRepository, IProductRepository productRepository)
        {
            _invoiceRepository = invoiceRepository;
            _priceRepository = priceRepository;
            _stockRepository = stockRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Retrieves all invoices.
        /// </summary>
        /// <returns>A view displaying all invoices.</returns>
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Invoice";

            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            return View("Index", invoices);
        }

        /// <summary>
        /// Retrieves the details of a specific invoice, including associated products and prices.
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice to retrieve details for.</param>
        /// <returns>A view displaying the product details associated with the invoice.</returns>
        public async Task<IActionResult> InvoiceDetails(int invoiceId)
        {
            var priceIds = await _invoiceRepository.GetAllPricesForInvoiceAsync(invoiceId);

            ViewData["InvoiceId"] = invoiceId;

            if (priceIds == null || !priceIds.Any())
            {
                ViewData["Message"] = "No products in this invoice.";
                return View(new List<InvoiceProductDetails>());
            }

            // Retrieve product details (name, price, quantity)
            var productDetails = await _priceRepository.GetInvoiceProductDetailsAsync(priceIds);

            return View(productDetails);
        }

        /// <summary>
        /// Accepts an invoice by adding its products to the stock and cleaning up invoice data.
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice to accept.</param>
        /// <returns>A view displaying all invoices after processing the accepted invoice.</returns>
        public async Task<IActionResult> AcceptInvoice(int invoiceId)
        {
            var priceIds = await _invoiceRepository.GetAllPricesForInvoiceAsync(invoiceId);
            if (priceIds == null || !priceIds.Any())
            {
                ViewData["Message"] = "Error: No product IDs found in the Price table for this invoice.";
                return View("InvoiceDetails", new List<InvoiceProductDetails>());
            }

            await _stockRepository.AddProductsByIds(priceIds);
            await CleanUpInvoices(invoiceId);

            ViewData["Message"] = "Invoice successfully accepted, and products added to Stock.";
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            return View("Index", invoices);
        }

        /// <summary>
        /// Discards an invoice by removing its products from the stock and deleting invoice-related data.
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice to discard.</param>
        /// <returns>A view displaying all invoices after processing the discarded invoice.</returns>
        public async Task<IActionResult> DiscardInvoice(int invoiceId)
        {
            var priceIds = await _invoiceRepository.GetAllPricesForInvoiceAsync(invoiceId);
            var productIds = await _priceRepository.GetProductIdsByPriceIds(priceIds);
            await CleanUpInvoices(invoiceId);
            await _priceRepository.DeleteManyByIdsAsync(priceIds);
            await _productRepository.DeleteManyByIdsAsync(productIds);

            ViewData["Message"] = "Invoice successfully discarded.";
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            return View("Index", invoices);
        }

        /// <summary>
        /// Cleans up invoice-related data by deleting the invoice and its associated prices.
        /// </summary>
        /// <param name="invoiceId">The ID of the invoice to delete.</param>
        private async Task CleanUpInvoices(int invoiceId)
        {
            await _invoiceRepository.DeleteInvoicePricesAsync(invoiceId);
            await _invoiceRepository.DeleteAsync(invoiceId);
        }
    }

}
