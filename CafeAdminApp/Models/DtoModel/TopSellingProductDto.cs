namespace CafeAdminApp.Models.DtoModel
{
    public class TopSellingProductDto
    {
        public string ProductName { get; set; } = string.Empty; // Назва товару
        public string CategoryName { get; set; } = string.Empty; // Категорія товару
        public int TotalSold { get; set; } // Загальна кількість проданих одиниць
    }

}
