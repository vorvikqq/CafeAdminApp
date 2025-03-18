namespace CafeAdminApp.Models.DtoModel
{
    public class StockItemDto
    {
        public string ProductName { get; set; } = string.Empty; // Назва товару
        public string CategoryName { get; set; } = string.Empty; // Категорія товару
        public int Quantity { get; set; } // Залишок на складі
        public bool IsSpoiled { get; set; } // Чи товар зіпсований (true - так, false - ні)
    }

}
