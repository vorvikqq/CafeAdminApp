using Microsoft.AspNetCore.Mvc;

namespace CafeAdminApp.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Index()
        {
            // TODO: отримуємо інфу про таблицю інвойси, передаємо у вью
            throw new NotImplementedException();
        }

        public IActionResult InvoiceDetails(int invoiceId)
        {
            // TODO: отримуємо інфу про продукт для цього інвойсу з таких таблиць:
            // Назва:Price->Product; Ціна:Price; Кількість:Price->Product.
            // Формуємо модель з цих трьох приколів та передаємо у вью
            // (Можна зробити кастомну модель для цього у папці Data) 
            throw new NotImplementedException();
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
