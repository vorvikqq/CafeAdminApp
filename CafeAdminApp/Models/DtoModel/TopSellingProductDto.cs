namespace CafeAdminApp.Models.DtoModel
{
    public class TopSellingProductDto
    {
        public string ProductName { get; set; } = string.Empty; 
        public string CategoryName { get; set; } = string.Empty;
        public int TotalSold { get; set; } 
    }

}
