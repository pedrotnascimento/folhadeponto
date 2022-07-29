using Microsoft.AspNetCore.Mvc;

namespace FolhaDePonto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FolhaDePontoController : ControllerBase
    {
     
        private readonly ILogger<FolhaDePontoController> _logger;

        public FolhaDePontoController(ILogger<FolhaDePontoController> logger)
        { 
            _logger = logger;
        }

    }
}