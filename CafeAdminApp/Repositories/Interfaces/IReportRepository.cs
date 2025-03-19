using CafeAdminApp.Models.DtoModel;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<decimal> GetNetProfitReport(DateTime startDate, DateTime endDate);
        Task<List<TopSellingProductDto>> GetTopSellingProducts(DateTime startDate, DateTime endDate);
        Task<List<StockItemDto>> GetStockReport();
        Task<List<SpoiledGoodsDto>> GetSpoiledProductsReport();
    }

}
