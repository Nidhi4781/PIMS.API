using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PIMS.allsoft.Context;
using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;

namespace PIMS.allsoft.Services
{
    public class InventoryService: IInventoryService
    {
        private readonly PIMSContext _context;

        public InventoryService(PIMSContext context)
        {
            _context = context;
        }

        public async Task<Inventory> AdjustInventoryAsync(InventoryAdjustment adjustment)
        {
            
            var inventory = new Inventory
            {
                ProductID = adjustment.ProductID,
                Quantity = adjustment.Quantity,
                WarehouseLocation = "Default Location", 
                Timestamp = DateTime.UtcNow,
                Reason = adjustment.Reason,
                UserResponsible = adjustment.UserResponsible
            };

            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return inventory;
        }

        public async Task<IEnumerable<Inventory>> GetLowInventoryAsync(int threshold)
        {
            return await _context.Inventories
                .Where(i => i.Quantity <= threshold)
                .ToListAsync();
        }

        public async Task<Inventory> AuditInventoryAsync(int inventoryId, int newQuantity, string reason,int userResponsible)
        {
           // Int32 UserID = HttpContext.Session.GetInt32("ID");

            var inventory = await _context.Inventories.FindAsync(inventoryId);

            if (inventory == null)
            {
                return null;
            }

            inventory.Quantity = newQuantity;
            inventory.Timestamp = DateTime.UtcNow;
            inventory.Reason = reason;
            inventory.UserResponsible = userResponsible;

            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();

            return inventory;
        }
    }
}
