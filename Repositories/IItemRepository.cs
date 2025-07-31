using Dapper;
using System.Data;
using TasteTrack_RMS.Models;

namespace TasteTrack_RMS.Repositories
{
    public class ItemRepository : BaseRepository, IItemRepository
    {
        public ItemRepository(IConfiguration configuration, ILogger<ItemRepository> logger)
            : base(configuration, logger) { }

        public async Task<List<ItemMaster>> GetAllItemsAsync()
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@action", "itemselectall");

                var items = await connection.QueryAsync<ItemMaster>(
                    "sp_rms_item", parameters, commandType: CommandType.StoredProcedure);
                return items.ToList();
            }, "Get All Items");
        }

        public async Task<ItemMaster?> GetItemByIdAsync(int itemId)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", itemId);
                parameters.Add("@action", "itemselectone");

                var item = await connection.QueryFirstOrDefaultAsync<ItemMaster>(
                    "sp_rms_item", parameters, commandType: CommandType.StoredProcedure);
                return item;
            }, "Get Item By ID");
        }

        public async Task<bool> CreateItemAsync(ItemMaster item)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@itemname", item.ItemName);
                parameters.Add("@price", item.ItemPrice);
                parameters.Add("@action", "iteminsert");

                var result = await connection.ExecuteAsync(
                    "sp_rms_item", parameters, commandType: CommandType.StoredProcedure);
                return result >= 0;
            }, "Create Item");
        }

        public async Task<bool> UpdateItemAsync(ItemMaster item)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", item.ItemID);
                parameters.Add("@itemname", item.ItemName);
                parameters.Add("@price", item.ItemPrice);
                parameters.Add("@action", "itemupdate");

                var result = await connection.ExecuteAsync(
                    "sp_rms_item", parameters, commandType: CommandType.StoredProcedure);
                return result > 0;
            }, "Update Item");
        }

        public async Task<bool> DeleteItemAsync(int itemId)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", itemId);
                parameters.Add("@action", "itemdelete");

                var result = await connection.ExecuteAsync(
                    "sp_rms_item", parameters, commandType: CommandType.StoredProcedure);
                return result > 0;
            }, "Delete Item");
        }

        public async Task<List<DailyItem>> GetDailyItemsAsync()
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@action", "dailyitemselect");

                // Use your stored procedure instead of direct SQL
                var items = await connection.QueryAsync<DailyItem>(
                    "sp_rms_item", parameters, commandType: CommandType.StoredProcedure);
                return items.ToList();
            }, "Get Daily Items");
        }

        public async Task<bool> AddDailyItemAsync(int itemId)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", itemId);
                parameters.Add("@action", "dailyiteminsert");

                var result = await connection.ExecuteAsync(
                    "sp_rms_item", parameters, commandType: CommandType.StoredProcedure);
                return result >= 0;
            }, "Add Daily Item");
        }

        public async Task<bool> UpdateDailyItemStatusAsync(int dailyItemId)
        {
            return await ExecuteWithExceptionHandlingAsync(async () =>
            {
                using var connection = GetConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", dailyItemId);
                parameters.Add("@action", "dailyitemupdate");

                var result = await connection.ExecuteAsync(
                    "sp_rms_item", parameters, commandType: CommandType.StoredProcedure);
                return result > 0;
            }, "Update Daily Item Status");
        }
    }
}
