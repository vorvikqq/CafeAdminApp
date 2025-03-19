namespace CafeAdminApp.Models
{
    /// <summary>
    /// Модель для зберігання данних які потрібні для представлення інвойсу
    /// </summary>
    public class InvoiceProductDetails
    {
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }

}
