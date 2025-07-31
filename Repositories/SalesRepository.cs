using Dapper;
using System.Data;
using TasteTrack_RMS.Models;

namespace TasteTrack_RMS.Repositories
{
    public class SalesRepository : BaseRepository, ISalesRepository
    {
        public SalesRepository(IConfiguration configuration, ILogger<SalesRepository> logger)
            : base(configuration, logger) { }

        public async Task<bool> AddItemToSaleAsync(int itemId, int quantity)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@itemid", itemId);
                parameters.Add("@quantity", quantity);
                parameters.Add("@action", "itemtosale");

                var result = await connection.ExecuteAsync(
                    "sp_rms_sales", parameters, commandType: CommandType.StoredProcedure);

                _logger.LogInformation($"Item {itemId} (qty: {quantity}) added to current sale");
                return result >= 0;
            }, "Add Item To Sale");
        }

        public async Task<bool> RemoveItemFromSaleAsync(int itemId)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@itemid", itemId);
                parameters.Add("@action", "removeitemfromsale");

                var result = await connection.ExecuteAsync(
                    "sp_rms_sales", parameters, commandType: CommandType.StoredProcedure);

                _logger.LogInformation($"Item {itemId} removed from current sale");
                return result >= 0;
            }, "Remove Item From Sale");
        }

        public async Task<bool> CompleteSaleAsync()
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@action", "inserttosalesmaster");

                var result = await connection.ExecuteAsync(
                    "sp_rms_sales", parameters, commandType: CommandType.StoredProcedure);

                _logger.LogInformation("Sale completed and inserted into SalesMaster");
                return result >= 0;
            }, "Complete Sale");
        }

        public async Task<List<SalesSlave>> GetCurrentSaleItemsAsync()
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@action", "print");

                var items = await connection.QueryAsync<SalesSlave>(
                    "sp_rms_sales", parameters, commandType: CommandType.StoredProcedure);

                return items.ToList();
            }, "Get Current Sale Items");
        }

        public async Task<List<SalesMaster>> GetAllSalesAsync()
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var sales = await connection.QueryAsync<SalesMaster>(
                    "SELECT OrderID, OrderDate, TotalValue FROM SalesMaster ORDER BY OrderDate DESC");

                return sales.ToList();
            }, "Get All Sales");
        }

        public async Task<SalesMaster?> GetSaleByIdAsync(int orderId)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var sale = await connection.QueryFirstOrDefaultAsync<SalesMaster>(
                    "SELECT OrderID, OrderDate, TotalValue FROM SalesMaster WHERE OrderID = @orderId",
                    new { orderId });

                return sale;
            }, "Get Sale By ID");
        }

        public async Task<List<SalesSlave>> GetSaleItemsAsync(int orderId)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var query = @"
                    SELECT ss.ID, ss.OrderID, ss.ItemID, ss.Quantity, ss.Value, 
                           im.ItemName
                    FROM SalesSlave ss
                    INNER JOIN ItemMaster im ON ss.ItemID = im.ItemID
                    WHERE ss.OrderID = @orderId";

                var items = await connection.QueryAsync<SalesSlave>(query, new { orderId });
                return items.ToList();
            }, "Get Sale Items");
        }
    }
}
