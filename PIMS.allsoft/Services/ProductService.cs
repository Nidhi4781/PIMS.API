using Microsoft.EntityFrameworkCore;
using PIMS.allsoft.Context;
using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;

namespace PIMS.allsoft.Services;

public class ProductService : IProductService
{
    private readonly PIMSContext _context;

    public ProductService(PIMSContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products.Include(p => p.ProductCategories).ToListAsync();
    }
    public async Task<Product> CreateProductAsync(Products productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            SKU = productDto.SKU,
            CreatedDate = DateTime.UtcNow // Assuming UTC time for simplicity
        };
        foreach (var categoryId in productDto.CategoryIds)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                product.ProductCategories.Add(new ProductCategory { Product = product, Category = category });
            }
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }
    public async Task<Product> UpdateProductAsync(int productId, Products productDto)
    {
        var product = await _context.Products
            .Include(p => p.ProductCategories)
            .FirstOrDefaultAsync(p => p.ProductID == productId);

        if (product == null)
        {
            return null;
        }

        product.Name = productDto.Name;
        product.Description = productDto.Description;
        product.Price = productDto.Price;
        product.SKU = productDto.SKU;

        // Update the product categories
        var existingCategories = product.ProductCategories.ToList();
        foreach (var existingCategory in existingCategories)
        {
            _context.ProductCategories.Remove(existingCategory);
        }

        foreach (var categoryId in productDto.CategoryIds)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                product.ProductCategories.Add(new ProductCategory { Product = product, Category = category });
            }
        }

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return product;
    }
    public async Task<bool> IsSKUUniqueAsync(string sku)
    {
        return !(await _context.Products.AnyAsync(p => p.SKU == sku));
    }

    public async Task<List<Product>> AdjustPricesAsync(PriceAdjustment adjustmentDto)
    {
        var productsToUpdate = await _context.Products
            .Where(p => adjustmentDto.ProductIds.Contains(p.ProductID))
            .ToListAsync();

        foreach (var product in productsToUpdate)
        {
            if (adjustmentDto.IsPercentage)
            {
                product.Price *= (1 - (adjustmentDto.AdjustmentAmount / 100));
            }
            else
            {
                product.Price -= adjustmentDto.AdjustmentAmount;
            }
        }

        await _context.SaveChangesAsync();

        return productsToUpdate;
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Where(p => p.ProductCategories.Any(pc => pc.CategoryID == categoryId))
            .ToListAsync();
    }
    //public async Task<Product> CreateProductAsync(Product product)
    //{
    //    // Map productDto to Product entity and add it to the context
    //    var products = new Product { /* Map properties */ };
    //    _context.Products.Add(products);
    //    await _context.SaveChangesAsync();
    //    return products;
    //}

    ////public async Task<IEnumerable<Product>> GetProductByIdAsync(int id)
    ////{
    ////    return await _context.Products.Include(p => p.ProductCategories).ToListAsync();
    ////}
    //public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
    //{
    //    return await _context.Products
    //        .Where(p => p.ProductCategories.Any(pc => pc.CategoryID == categoryId))
    //        .ToListAsync();
    //}

    ////public async Task<List<Product>> GetProductsByCategoryNameAsync(string categoryName)
    ////{
    ////    return await _context.Products
    ////        .Where(p => p.ProductCategories.Any(pc => pc.Category.Name == categoryName))
    ////        .ToListAsync();
    ////}
    //// Implement other methods as needed
}
