using CafeAdminApp.Models.DtoModel;

namespace CafeAdminApp.Models.ViewModels
{
    public class ReportFilterViewModel
    {
        public string Filter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal NetProfit { get; set; }
        public List<TopSellingProductDto>? TopSellingProducts { get; set; }
        public List<StockItemDto>? StockAvailability { get; set; }
        public List<SpoiledGoodsDto>? SpoiledGoods { get; set; }

        public void SetDateRange()
        {
            DateTime today = DateTime.Today;

            // Якщо користувач задав власний діапазон, перевіряємо його валідність
            bool isCustomRange = StartDate != default && EndDate != default && StartDate <= EndDate;

            if (!isCustomRange) // Якщо власний діапазон не заданий або некоректний, встановлюємо за фільтром
            {
                switch (Filter)
                {
                    case "day":
                        StartDate = today;
                        EndDate = today;
                        break;
                    case "week":
                        StartDate = today.AddDays(-7);
                        EndDate = today;
                        break;
                    case "month":
                        StartDate = new DateTime(today.Year, today.Month, 1);
                        EndDate = StartDate.AddMonths(1).AddDays(-1);
                        break;
                    case "quarter":
                        int currentQuarter = (today.Month - 1) / 3 + 1;
                        StartDate = new DateTime(today.Year, (currentQuarter - 1) * 3 + 1, 1);
                        EndDate = StartDate.AddMonths(3).AddDays(-1);
                        break;
                    default:
                        // Якщо фільтр невідомий, встановлюємо поточну дату
                        StartDate = today;
                        EndDate = today;
                        break;
                }
            }
        }

    }
}
