using CafeAdminApp.Models;
using CafeAdminApp.Models.ViewModels;
using CafeAdminApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CafeAdminApp.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository _orderRepository;
        private IPriceRepository _priceRepository;
        private ICheckRepository _checkRepository;
        private IStockRepository _stockRepository;
        private IProductRepository _productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderController"/> class.
        /// </summary>
        /// <param name="orderRepository">Repository for handling order-related operations.</param>
        /// <param name="priceRepository">Repository for handling price-related operations.</param>
        /// <param name="checkRepository">Repository for managing sales checks.</param>
        /// <param name="stockRepository">Repository for managing stock data.</param>
        /// <param name="productRepository">Repository for managing product data.</param>
        public OrderController(IOrderRepository orderRepository, IPriceRepository priceRepository, ICheckRepository checkRepository, IStockRepository stockRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _priceRepository = priceRepository;
            _checkRepository = checkRepository;
            _stockRepository = stockRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Displays all unconfirmed orders.
        /// </summary>
        /// <returns>A view containing a list of unconfirmed orders.</returns>
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Orders";
            var orders = await _orderRepository.GetAllUnconfirmedAsync();

            return View("Index", orders);
        }

        /// <summary>
        /// Displays detailed information for a specific order, including products.
        /// </summary>
        /// <param name="orderId">The ID of the order to display details for.</param>
        /// <returns>A view with order product details.</returns>
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var priceIds = await _orderRepository.GetAllPricesForOrderAsync(orderId);

            ViewData["OrderId"] = orderId;

            if (priceIds == null || !priceIds.Any())
            {
                ViewData["Message"] = "No products found for this order.";
                return View(new List<OrderDetails>());
            }

            var productDetails = await _priceRepository.GetOrderDetailsAsync(priceIds, orderId);

            return View(productDetails);
        }

        /// <summary>
        /// Accepts an order by updating its status and generating a sales check.
        /// </summary>
        /// <param name="orderId">The ID of the order to accept.</param>
        /// <returns>A view displaying updated unconfirmed orders.</returns>
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            await _orderRepository.UpdateOrderStatus(orderId, true);
            var check = new Check()
            {
                OrderId = orderId,
                SaleDate = DateTime.Now
            };
            await _checkRepository.AddAsync(check);

            ViewData["Message"] = "Order accepted. Sales check successfully created.";
            var orders = await _orderRepository.GetAllUnconfirmedAsync();
            return View("Index", orders);
        }

        /// <summary>
        /// Discards an order by restoring its products to stock and deleting all associated records.
        /// </summary>
        /// <param name="orderId">The ID of the order to discard.</param>
        /// <returns>A view displaying updated unconfirmed orders.</returns>
        public async Task<IActionResult> DiscardOrder(int orderId)
        {
            var priceIds = await _orderRepository.GetAllPricesForOrderAsync(orderId);
            await _stockRepository.AddProductsByIdsFromOrder(priceIds, orderId);

            var controller = new ProductExpirationController(_stockRepository, _productRepository);
            var result = await controller.SetExpirationFlag();

            await _orderRepository.DeleteOrderPricesAsync(orderId);
            await _orderRepository.DeleteAsync(orderId);

            ViewData["Message"] = "Order successfully discarded.";
            var orders = await _orderRepository.GetAllUnconfirmedAsync();
            return View("Index", orders);
        }
    }

}
