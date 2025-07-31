using TasteTrack_RMS.Models;

namespace TasteTrack_RMS.Repositories
{
    public interface IItemRepository
    {
        Task<List<ItemMaster>> GetAllItemsAsync();
        Task<ItemMaster?> GetItemByIdAsync(int itemId);
        Task<bool> CreateItemAsync(ItemMaster item);
        Task<bool> UpdateItemAsync(ItemMaster item);
        Task<bool> DeleteItemAsync(int itemId);
        Task<List<DailyItem>> GetDailyItemsAsync();
        Task<bool> AddDailyItemAsync(int itemId);
        Task<bool> UpdateDailyItemStatusAsync(int dailyItemId);
    }
}
