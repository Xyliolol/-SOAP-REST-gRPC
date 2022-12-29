using Microsoft.AspNetCore.Mvc;
using SampleService.Services;

namespace SampleService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        //private RootServiceReference.RootServiceClient _httpClient;
        private IRootServiceClient _rootServiceClient;

        public WeatherForecastController(
            //IHttpClientFactory httpClientFactory,
            IRootServiceClient rootServiceClient,
            ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _rootServiceClient = rootServiceClient;
            //_httpClient = new RootServiceReference.RootServiceClient("http://localhost:5075/", 
            //    httpClientFactory.CreateClient("RootServiceClient"));
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<ActionResult<IEnumerable<RootServiceReference.WeatherForecast>>> Get()
        {
            return Ok(await _rootServiceClient.Get());
        }
    }
}