using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger, IMemoryCache memoryCache)
        {
            _productService = productService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Create a new product. Admin can access this API.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/v1.0/products/Create
        ///     {
        ///         "name": "Laptop",
        ///         "description": "High-performance laptop",
        ///         "price": 1200,
        ///         "sku": "LPT001",
        ///         "createdDate": "2024-06-20T17:25:08.702Z",
        ///         "categoryIds": [1, 2] // Category IDs
        ///     }
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     {
        ///         "productId": 1,
        ///         "name": "Laptop",
        ///         "description": "High-performance laptop",
        ///         "price": 1200,
        ///         "createdDate": "2024-06-20T17:25:08.702Z",
        ///         "sku": "LPT001",
        ///         "productCategories": [
        ///             {
        ///                 "productId": 1,
        ///                 "categoryId": 1
        ///             },
        ///             {
        ///                 "productId": 1,
        ///                 "categoryId": 2
        ///             }
        ///         ]
        ///     }
        /// 
        /// </remarks>
        /// <param name="Product">The product to create.</param>
        /// <response code="200">Returns the created product.</response>
        /// <response code="400">If the product is null or invalid.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("Create")]
        public async Task<IActionResult> CreateProduct(Products Product)
        {
            // Check SKU uniqueness before creating the product
            if (await _productService.IsSKUUniqueAsync(Product.SKU))
            {
                var createdProduct = await _productService.CreateProductAsync(Product);
                return Ok(createdProduct);
            }
            else
            {
                return BadRequest("SKU must be unique.");
            }
        }

        /// <summary>
        /// Update an existing product. Admin can access this API.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     PUT /api/v1.0/products/Update/1
        ///     {
        ///         "name": "Updated Laptop",
        ///         "description": "Updated high-performance laptop",
        ///         "price": 1500,
        ///         "sku": "LPT002",
        ///         "categoryIds": [1, 3] // Updated category IDs
        ///     }
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     {
        ///         "productId": 1,
        ///         "name": "Updated Laptop",
        ///         "description": "Updated high-performance laptop",
        ///         "price": 1500,
        ///         "createdDate": "2024-06-20T17:25:08.702Z",
        ///         "sku": "LPT002",
        ///         "productCategories": [
        ///             {
        ///                 "productId": 1,
        ///                 "categoryId": 1
        ///             },
        ///             {
        ///                 "productId": 1,
        ///                 "categoryId": 3
        ///             }
        ///         ]
        ///     }
        /// 
        /// </remarks>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="productDto">The updated product details.</param>
        /// <response code="200">Returns the updated product.</response>
        /// <response code="400">If the product ID is invalid or the SKU is not unique.</response>
        /// <response code="404">If the product to update is not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("Update/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, Products productDto)
        {
            // Check SKU uniqueness before updating the product
            if (await _productService.IsSKUUniqueAsync(productDto.SKU))
            {
                var updatedProduct = await _productService.UpdateProductAsync(productId, productDto);
                if (updatedProduct == null)
                {
                    return NotFound("Product not found.");
                }

                return Ok(updatedProduct);
            }
            else
            {
                return BadRequest("SKU must be unique.");
            }
        }

        /// <summary>
        /// Adjust prices of products. Admin can access this API.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/v1.0/products/AdjustPrice
        ///     {
        ///         "productIds": [1, 2, 3],
        ///         "adjustmentAmount": 10,
        ///         "isPercentage": true
        ///     }
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     [
        ///         {
        ///             "productId": 1,
        ///             "name": "Updated Laptop",
        ///             "description": "Updated high-performance laptop",
        ///             "price": 1350,
        ///             "createdDate": "2024-06-20T17:25:08.702Z",
        ///             "sku": "LPT002",
        ///             "productCategories": [
        ///                 {
        ///                     "productId": 1,
        ///                     "categoryId": 1
        ///                 },
        ///                 {
        ///                     "productId": 1,
        ///                     "categoryId": 3
        ///                 }
        ///             ]
        ///         },
        ///         ...
        ///     ]
        /// 
        /// </remarks>
        /// <param name="adjustmentDto">The price adjustment details.</param>
        /// <response code="200">Returns the adjusted products.</response>
        /// <response code="400">If the adjustment details are invalid.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("AdjustPrice")]
        public async Task<IActionResult> AdjustPrice(PriceAdjustment adjustmentDto)
        {
            // Implement price adjustment logic here
            var adjustedProducts = await _productService.AdjustPricesAsync(adjustmentDto);
            return Ok(adjustedProducts);
        }

        /// <summary>
        /// Filter products by category. Admin and User roles can access this API.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET /api/v1.0/products/FilterByCategory/1
        ///     
        /// Sample Response:
        /// 
        ///     200 OK
        ///     [
        ///         {
        ///             "productId": 1,
        ///             "name": "Updated Laptop",
        ///             "description": "Updated high-performance laptop",
        ///             "price": 1350,
        ///             "createdDate": "2024-06-20T17:25:08.702Z",
        ///             "sku": "LPT002",
        ///             "productCategories": [
        ///                 {
        ///                     "productId": 1,
        ///                     "categoryId": 1
        ///                 },
        ///                 {
        ///                     "productId": 1,
        ///                     "categoryId": 3
        ///                 }
        ///             ]
        ///         },
        ///         ...
        ///     ]
        /// 
        /// </remarks>
        /// <param name="categoryId">The ID of the category to filter by.</param>
        /// <response code="200">Returns the filtered products.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("FilterByCategory/{categoryId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> FilterByCategory(int categoryId)
        {
            // Get products by category ID
            var filteredProducts = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(filteredProducts);
        }

        /// <summary>
        /// Get all products. Admin and User roles can access this API.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET /api/v1.0/products/AllProducts
        ///     
        /// Sample Response:
        /// 
        ///     200 OK
        ///     [
        ///         {
        ///             "productId": 1,
        ///             "name": "Product Name",
        ///             "description": "Product Description",
        ///             "price": 100,
        ///             "createdDate": "2024-06-21T14:30:00Z",
        ///             "sku": "PROD001",
        ///             "productCategories": [
        ///                 {
        ///                     "productId": 1,
        ///                     "categoryId": 1
        ///                 },
        ///                 ...
        ///             ]
        ///         },
        ///         ...
        ///     ]
        /// 
        /// </remarks>
        /// <response code="200">Returns the list of all products.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("AllProducts")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

      
        //[HttpGet("category/{categoryId}")]
        //public async Task<IActionResult> GetProductsByCategory(int categoryId)
        //{
        //    var products = await _productService.GetProductsByCategoryAsync(categoryId);
        //    if (products == null || products.Count == 0)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(products);
        //}
     
    }
}
