using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;
using System.Data;

namespace PIMS.allsoft.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [Authorize(Roles = "Admin")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        [HttpPost("Adjust")]
        public async Task<IActionResult> AdjustInventory(InventoryAdjustment adjustment)
        {
            var result = await _inventoryService.AdjustInventoryAsync(adjustment);
            return Ok(result);
        }

        [HttpGet("LowInventory")]
        public async Task<IActionResult> GetLowInventory(int threshold)
        {
            var result = await _inventoryService.GetLowInventoryAsync(threshold);
            return Ok(result);
        }

        [HttpPost("Audit/{inventoryId}")]
        public async Task<IActionResult> AuditInventory(int inventoryId, int newQuantity, string reason, int userResponsible)
        {
            var result = await _inventoryService.AuditInventoryAsync(inventoryId, newQuantity, reason, userResponsible);
            if (result == null)
            {
                return NotFound("Inventory item not found.");
            }

            return Ok(result);
        }
    }
}
