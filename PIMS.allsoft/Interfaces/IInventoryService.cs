using PIMS.allsoft.Models;

namespace PIMS.allsoft.Interfaces
{
    public interface IInventoryService
    {
        Task<Inventory> AdjustInventoryAsync(InventoryAdjustment adjustment);
        Task<IEnumerable<Inventory>> GetLowInventoryAsync(int threshold);
        Task<Inventory> AuditInventoryAsync(int inventoryId, int newQuantity, string reason, int userResponsible);
    }
}
