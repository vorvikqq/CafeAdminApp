using CafeAdminApp.Models.DtoModel;

namespace CafeAdminApp.Models.ViewModels
{
    public class ReportFilterViewModel
    {
        public string Filter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<NetProfitDto> NetProfitDetails { get; set; } = new();
        public List<TopSellingProductDto>? TopSellingProducts { get; set; }
        public List<StockItemDto>? StockAvailability { get; set; }
        public List<SpoiledGoodsDto>? SpoiledGoods { get; set; }

        public void SetDateRange()
        {
            DateTime today = DateTime.Today;

            // Якщо користувач задав власний діапазон, перевіряємо його валідність
            bool isCustomRange = StartDate != default && EndDate != default && StartDate <= EndDate;
        }

    }
}
