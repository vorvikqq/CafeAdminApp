namespace CafeAdminApp.Models.DtoModel
{
    public class SpoiledGoodsDto
    {
        public string ProductName { get; set; } = string.Empty; // Назва товару
        public int TotalQuantity { get; set; } // Загальна кількість зіпсованих товарів
        public decimal TotalCost { get; set; } // Загальна сума списаних товарів
    }

}
