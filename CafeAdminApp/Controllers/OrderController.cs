using CafeAdminApp.Models;
using CafeAdminApp.Models.ViewModels;
using CafeAdminApp.Repositories;
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
        public OrderController(IOrderRepository orderRepository, IPriceRepository priceRepository, ICheckRepository checkRepository, IStockRepository stockRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _priceRepository = priceRepository;
            _checkRepository = checkRepository;
            _stockRepository = stockRepository;
            _productRepository = productRepository;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["ActivePage"] = "Orders";
            var orders = await _orderRepository.GetAllUnconfirmedAsync();

            return View("Index", orders);
        }

        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var priceIds = await _orderRepository.GetAllPricesForOrderAsync(orderId);

            ViewData["OrderId"] = orderId;

            if (priceIds == null || !priceIds.Any())
            {
                ViewData["Message"] = "Немає товарів у цьому замовленні.";
                return View(new List<OrderDetails>());
            }

            var productDetails = await _priceRepository.GetOrderDetailsAsync(priceIds, orderId);

            return View(productDetails);
        }

        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            await _orderRepository.UpdateOrderStatus(orderId, true);
            var check = new Check()
            {
                OrderId = orderId,
                SaleDate = DateTime.Now
            };
            await _checkRepository.AddAsync(check);

            ViewData["Message"] = "Замовлення прийнято. Чек успішно додано";
            var orders = await _orderRepository.GetAllUnconfirmedAsync();
            return View("Index", orders);

        }

        public async Task<IActionResult> DiscardOrder(int orderId)
        {
            // повертаємо продукти в Stock
            var priceIds = await _orderRepository.GetAllPricesForOrderAsync(orderId);
            await _stockRepository.AddProductsByIdsFromOrder(priceIds, orderId);

            var controller = new ProductExpirationController(_stockRepository, _productRepository);
            var result = await controller.SetExpirationFlag();

            // видаляємо всі записи в ордер прайс
            await _orderRepository.DeleteOrderPricesAsync(orderId);
            // видаляємо всі записи в ордер (1)
            await _orderRepository.DeleteAsync(orderId);

            ViewData["Message"] = "Замовлення успішно відхилено.";
            var orders = await _orderRepository.GetAllUnconfirmedAsync();
            return View("Index", orders);
        }
    }
}
