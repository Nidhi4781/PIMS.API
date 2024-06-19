using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;
using System.Data;

namespace PIMS.allsoft.Controllers.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        [HttpPost("Adjust")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AdjustInventory(InventoryAdjustment adjustment)
        {
            var result = await _inventoryService.AdjustInventoryAsync(adjustment);
            return Ok(result);
        }

        [HttpGet("LowInventory")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetLowInventory(int threshold)
        {
            var result = await _inventoryService.GetLowInventoryAsync(threshold);
            return Ok(result);
        }

        [HttpPost("Audit/{inventoryId}")]
        [Authorize(Roles = "User")]
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
