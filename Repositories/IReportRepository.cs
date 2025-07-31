using TasteTrack_RMS.Models;

namespace TasteTrack_RMS.Repositories
{
    public interface IReportRepository
    {
        Task<List<ItemWiseReportData>> GetDailyItemWiseReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<SalesMaster>> GetDailyOrderWiseReportAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<SalesSlave>> GetSalesReportAsync(DateTime startDate, DateTime endDate);
        Task<List<DailySalesReport>> GetSalesComparisonReportAsync(DateTime startDate, DateTime endDate);
    }
}
