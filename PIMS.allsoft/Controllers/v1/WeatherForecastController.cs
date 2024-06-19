using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PIMS.allsoft.Models;

namespace PIMS.allsoft.Controllers.v1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0",Deprecated =true)]
    [Authorize]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        //[Authorize(Roles = "Admin")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "User")]
        public ActionResult<Product> Get(int id)
        {
            if (_memoryCache.TryGetValue($"Product_{id}", out Product cachedProduct))
            {
                return Ok(cachedProduct);
            }
            else
            {
                var product = new Product { ProductID = id, Name = $"Product {id}", Price = 99.99M };

                _memoryCache.Set($"Product_{id}", product, TimeSpan.FromMinutes(1));

                return Ok(product);
            }
        }
    }
}