using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PIMS.allsoft.Interfaces;
using PIMS.allsoft.Models;
using PIMS.allsoft.Services;

namespace PIMS.allsoft.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [Authorize(Roles ="Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService,ILogger<WeatherForecastController> logger, IMemoryCache memoryCache)
        {
            _categoryService = categoryService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
           // var categories = await _categoryService.GetAllCategoriesAsync();
            var categories = await _memoryCache.GetOrCreateAsync("Categories", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // Cache duration

                var fetchedCategories = await _categoryService.GetAllCategoriesAsync();
                return fetchedCategories;
            });
            return Ok(categories);
        }
        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync(Categories Category)
        {
            var category = await _categoryService.AddCategoryAsync(Category);

            return Created("", category); // Or Ok(category); if you prefer using 200 status code.

            //var Categorys = await _categoryService.AddCategoryAsync(Category);
            //return (IActionResult)Categorys;
        }
        // Other CRUD endpoints for categories
    }
}
