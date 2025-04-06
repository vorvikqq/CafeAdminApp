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
        public OrderController(IOrderRepository orderRepository, IPriceRepository priceRepository)
        {
            _orderRepository = orderRepository;
            _priceRepository = priceRepository;
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
            // оновляємо статус замовлення на тру
            // додаєємо в таблицю чек під'єднання до таблиці ордер по айді

            
            ViewData["Message"] = "unfinished/ Замовлення прийнято. Чек успішно додано";
            var orders = await _orderRepository.GetAllUnconfirmedAsync();
            return View("Index", orders);

        }

        /// <summary>
        /// НЕ додавати продукти в Stock, видалити продукти які були у інвойсів, видалити інформацію про інвойс
        /// </summary>
        /// <param name="invoiceId"> інвойс продукти якого видаляються </param>
        /// <returns></returns>
        public async Task<IActionResult> DiscardOrder(int orderId)
        {
            // додаємо в сток всі продукти із ордер прайс
            // видаляємо всі записи в ордер прайс
            // видаляємо всі записи в ордер (1)

            ViewData["Message"] = "unfinished/ Замовлення успішно відхилено.";
            var orders = await _orderRepository.GetAllUnconfirmedAsync();
            return View("Index", orders);
        }
    }
}
