using PIMS.allsoft.Models;

namespace PIMS.allsoft.Interfaces;

public interface IProductService
{

    Task<Product> CreateProductAsync(Products Products);
    Task<Product> UpdateProductAsync(int productId,Products Products);
    Task<bool> IsSKUUniqueAsync(string sku);
    Task<List<Product>> AdjustPricesAsync(PriceAdjustment adjustmentDto);
    Task<List<Product>> GetProductsByCategoryAsync(int categoryId);

    /// <summary>
    /// //
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<Product>> GetAllProductsAsync();
   // Task<Product> CreateProductAsync(Product productDto);
   //// Task<IEnumerable<Product>> GetProductByIdAsync(int id);
   // Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
   //// Task<List<Product>> GetProductsByCategoryNameAsync(string categoryName);
   // // Other methods for product-related operations
}
