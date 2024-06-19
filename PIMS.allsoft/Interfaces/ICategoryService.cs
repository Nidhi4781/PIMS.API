using PIMS.allsoft.Models;

namespace PIMS.allsoft.Interfaces
{

    public interface ICategoryService
    {
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
      //  Task AddCategoryAsync(Category category);
        Task<Category> AddCategoryAsync(Categories Category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int categoryId);
        // Other methods for category-related operations
    }
}