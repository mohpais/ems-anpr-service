using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Lonsum.Services.ANPR.Application.Events;

namespace Microsoft.Lonsum.Services.ANPR.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventBus _eventBus;
        public HomeController(IEventBus eventBus)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }

        [Route("TestRabbitMq")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _eventBus.Exchange("hello");
            return Ok();
        }
    }
}
