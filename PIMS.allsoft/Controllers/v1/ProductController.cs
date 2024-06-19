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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

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
        [HttpPost("AdjustPrice")]
        public async Task<IActionResult> AdjustPrice(PriceAdjustment adjustmentDto)
        {
            // Implement price adjustment logic here
            var adjustedProducts = await _productService.AdjustPricesAsync(adjustmentDto);
            return Ok(adjustedProducts);
        }

        [HttpGet("FilterByCategory/{categoryId}")]
        public async Task<IActionResult> FilterByCategory(int categoryId)
        {
            // Get products by category ID
            var filteredProducts = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(filteredProducts);
        }
        /// <summary>
        /// ///
        /// </summary>
        /// <returns></returns>
        /// 

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateProduct(Product productDto)
        //{
        //    var createdProduct = await _productService.CreateProductAsync(productDto);
        //    return (IActionResult)createdProduct;
        //}
        //[HttpPost]
        //public async Task<IActionResult> GetProductByIdAsync(int id)
        //{
        //    Product createdProduct = await _productService.GetProductByIdAsync(id);
        //    return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductID }, createdProduct);
        //}
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            if (products == null || products.Count == 0)
            {
                return NotFound();
            }
            return Ok(products);
        }
        //[HttpGet("category")]
        //public async Task<IActionResult> GetProductsByCategoryName(string categoryName)
        //{
        //    var products = await _productService.GetProductsByCategoryNameAsync(categoryName);
        //    if (products == null || products.Count == 0)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(products);
        //}
        // Other CRUD endpoints for products
    }
}
