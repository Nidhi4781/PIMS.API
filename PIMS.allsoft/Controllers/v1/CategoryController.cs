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
    [Consumes("application/json")] //only accept `application/json`
    public class CategoryController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService,ILogger<CategoryController> logger, IMemoryCache memoryCache)
        {
            _categoryService = categoryService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Get all categories.Admin can access this API.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     GET /api/v1.0/categories
        ///     
        /// Sample Response:
        /// 
        ///     200 OK
        ///     [
        ///         {
        ///             "categoryID": 1,
        ///             "name": "Electronics"
        ///         },
        ///         {
        ///             "categoryID": 2,
        ///             "name": "Books"
        ///         }
        ///     ]
        /// 
        /// </remarks>
        /// <response code="200">Returns the list of categories.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("AllCategories")]
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

        /// <summary>
        /// Add a new category. Admin can access this API.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     POST /api/v1.0/categories
        ///     {
        ///         "categoryID": 1,
        ///         "name": "Electronics"
        ///     }
        /// 
        /// Sample Response:
        /// 
        ///     201 Created
        ///     {
        ///         "categoryID": 1,
        ///         "name": "Electronics"
        ///     }
        /// 
        /// </remarks>
        /// <param name="Category">The category to add.</param>
        /// <response code="201">Returns the created category.</response>
        /// <response code="400">If the category is null or invalid.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("AddCategory")]
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
