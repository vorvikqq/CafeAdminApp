namespace CafeAdminApp.Models
{
    public class StockItem
    {
        public int StockID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public bool IsProsrochka { get; set; }
    }
}
