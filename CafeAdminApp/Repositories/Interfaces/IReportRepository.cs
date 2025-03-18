using CafeAdminApp.Models.DtoModel;

namespace CafeAdminApp.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<decimal> GetNetProfitAsync(DateTime startDate, DateTime endDate);
        Task<List<TopSellingProductDto>> GetBestSellingProductsAsync(DateTime startDate, DateTime endDate);
        Task<List<StockItemDto>> GetStockAvailabilityAsync();
        Task<List<SpoiledGoodsDto>> GetSpoiledGoodsAsync();
    }

}
