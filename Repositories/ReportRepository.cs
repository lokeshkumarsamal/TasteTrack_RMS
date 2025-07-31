using Dapper;
using System.Data;
using TasteTrack_RMS.Models;

namespace TasteTrack_RMS.Repositories
{
    public class ReportRepository : BaseRepository, IReportRepository
    {
        public ReportRepository(IConfiguration configuration, ILogger<ReportRepository> logger)
            : base(configuration, logger) { }

        public async Task<List<ItemWiseReportData>> GetDailyItemWiseReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@stdate", startDate?.Date);
                parameters.Add("@enddate", endDate?.Date);
                parameters.Add("@action", "dsritemwise");

                var report = await connection.QueryAsync<ItemWiseReportData>(
                    "sp_rms_report", parameters, commandType: CommandType.StoredProcedure);

                return report.ToList();
            }, "Get Daily Item Wise Report");
        }

        public async Task<List<SalesMaster>> GetDailyOrderWiseReportAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@stdate", startDate?.Date);
                parameters.Add("@enddate", endDate?.Date);
                parameters.Add("@action", "dsrorderwise");

                var report = await connection.QueryAsync<SalesMaster>(
                    "sp_rms_report", parameters, commandType: CommandType.StoredProcedure);

                return report.ToList();
            }, "Get Daily Order Wise Report");
        }

        public async Task<List<SalesSlave>> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@stdate", startDate.Date);
                parameters.Add("@enddate", endDate.Date);
                parameters.Add("@action", "salesreport");

                var report = await connection.QueryAsync<SalesSlave>(
                    "sp_rms_report", parameters, commandType: CommandType.StoredProcedure);

                return report.ToList();
            }, "Get Sales Report");
        }

        public async Task<List<DailySalesReport>> GetSalesComparisonReportAsync(DateTime startDate, DateTime endDate)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@stdate", startDate.Date);
                parameters.Add("@enddate", endDate.Date);
                parameters.Add("@action", "salescompreport");

                // Map to DailySalesReport with TotalValue property
                var report = await connection.QueryAsync<DailySalesReport>(
                    "sp_rms_report", parameters, commandType: CommandType.StoredProcedure);

                return report.ToList();
            }, "Get Sales Comparison Report");
        }

    }
}
