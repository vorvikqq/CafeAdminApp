namespace CafeAdminApp.Models.DtoModel
{
    public class StockItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsSpoiled { get; set; }
    }

}
