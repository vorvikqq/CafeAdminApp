namespace CafeAdminApp.Models.DtoModel
{
    public class NetProfitDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal BoughtPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal TotalProfit { get; set; }
    }

}
