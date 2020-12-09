using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SampleWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> _logger;
        
        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation("Get was called");
            return $"[{Activity.Current?.Id}] {User.Identity?.Name}";
        }
    }
}