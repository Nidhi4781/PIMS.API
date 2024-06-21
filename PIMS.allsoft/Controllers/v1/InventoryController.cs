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
    [Consumes("application/json")] //only accept `application/json`
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Adjust the inventory for a product. Admin can access this API and User roles can access this API with v2.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/v1.0/products/Adjust
        ///     {
        ///         "productId": 1,
        ///         "quantity": 50,
        ///         "reason": "Stock replenishment",
        ///         "userResponsible": 101
        ///     }
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     {
        ///         "inventoryId": 1,
        ///         "productId": 1,
        ///         "quantity": 50,
        ///         "warehouseLocation": "Default Location",
        ///         "timestamp": "2024-06-21T14:30:00Z",
        ///         "reason": "Stock replenishment",
        ///         "userResponsible": 101
        ///     }
        /// 
        /// </remarks>
        /// <param name="adjustment">The inventory adjustment details.</param>
        /// <response code="200">Returns the adjusted inventory.</response>
        /// <response code="400">If the adjustment details are invalid.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("Adjust")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdjustInventory(InventoryAdjustment adjustment)
        {
            var result = await _inventoryService.AdjustInventoryAsync(adjustment);
            return Ok(result);
        }

        /// <summary>
        /// Get products with low inventory. Admin can access this API. User roles can access this API with v2
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET /api/v1.0/products/LowInventory?threshold=10
        ///     
        /// Sample Response:
        /// 
        ///     200 OK
        ///     [
        ///         {
        ///             "inventoryId": 1,
        ///             "productId": 1,
        ///             "quantity": 5,
        ///             "warehouseLocation": "Default Location",
        ///             "timestamp": "2024-06-21T14:30:00Z",
        ///             "reason": "Low stock",
        ///             "userResponsible": 101,
        ///             "product": {
        ///                 "productId": 1,
        ///                 "name": "Product Name",
        ///                 "description": "Product Description",
        ///                 "price": 100
        ///             }
        ///         },
        ///         ...
        ///     ]
        /// 
        /// </remarks>
        /// <param name="threshold">The threshold for low inventory.</param>
        /// <response code="200">Returns the products with low inventory.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("LowInventory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLowInventory(int threshold)
        {
            var result = await _inventoryService.GetLowInventoryAsync(threshold);
            return Ok(result);
        }

        /// <summary>
        /// Audit the inventory for a specific product. Admin can access this API. User roles can access this API with v2
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/v1.0/products/Audit/1
        ///     {
        ///         "newQuantity": 30,
        ///         "reason": "Correction",
        ///         "userResponsible": 101
        ///     }
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     {
        ///         "inventoryId": 1,
        ///         "productId": 1,
        ///         "quantity": 30,
        ///         "warehouseLocation": "Default Location",
        ///         "timestamp": "2024-06-21T14:30:00Z",
        ///         "reason": "Correction",
        ///         "userResponsible": 101
        ///     }
        /// 
        /// </remarks>
        /// <param name="inventoryId">The ID of the inventory to audit.</param>
        /// <param name="newQuantity">The new quantity after the audit.</param>
        /// <param name="reason">The reason for the audit.</param>
        /// <param name="userResponsible">The user responsible for the audit.</param>
        /// <response code="200">Returns the audited inventory.</response>
        /// <response code="400">If the audit details are invalid.</response>
        /// <response code="404">If the inventory item is not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("Audit/{inventoryId}")]
        [Authorize(Roles = "Admin")]
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
