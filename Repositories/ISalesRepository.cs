using TasteTrack_RMS.Models;

namespace TasteTrack_RMS.Repositories
{
    public interface ISalesRepository
    {
        Task<bool> AddItemToSaleAsync(int itemId, int quantity);
        Task<bool> RemoveItemFromSaleAsync(int itemId);
        Task<bool> CompleteSaleAsync();
        Task<List<SalesSlave>> GetCurrentSaleItemsAsync();
        Task<List<SalesMaster>> GetAllSalesAsync();
        Task<SalesMaster?> GetSaleByIdAsync(int orderId);
        Task<List<SalesSlave>> GetSaleItemsAsync(int orderId);
    }
}
