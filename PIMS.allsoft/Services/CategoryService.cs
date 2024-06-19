using Microsoft.EntityFrameworkCore;
using PIMS.allsoft.Context;
using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;

namespace PIMS.allsoft.Services;

public class CategoryService : ICategoryService
{
    private readonly PIMSContext _context;

    public CategoryService(PIMSContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }
    public async Task<Category> GetCategoryByIdAsync(int categoryId)
    {
        return await _context.Categories.FindAsync(categoryId);
    }

    //public async Task AddCategoryAsync(Category category)
    //{
    //    _context.Categories.Add(category);
    //    await _context.SaveChangesAsync();
    //}
    public async Task<Category> AddCategoryAsync(Categories Category)
    {
        // Map productDto to Product entity and add it to the context
        var Categorys = new Category
        {
            CategoryID=Category.CategoryID,
            Name= Category.Name
        };

        _context.Categories.Add(Categorys);
        await _context.SaveChangesAsync();
        return Categorys;
    }
    public async Task UpdateCategoryAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        var category = await _context.Categories.FindAsync(categoryId);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
    // Implement other methods as needed
}
