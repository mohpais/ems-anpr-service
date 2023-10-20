using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Lonsum.Services.ANPR.API.Extensions;
using Microsoft.Lonsum.Services.ANPR.API.Models;
using Microsoft.Lonsum.Services.ANPR.API.ViewModels;
using System.Xml.Serialization;

namespace Microsoft.Lonsum.Services.ANPR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlateNumberController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        public PlateNumberController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("ANPRClient");
        }

        [Route("Get")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var request = "/ISAPI/Traffic/MNPR/channels/1?laneNo=1";
            try
            {
                var response = await _httpClient.GetMultipartAsync(request);
                if (response != null)
                {
                    // Manage data XML from Response
                    var xmlData = new EventNotificationAlert();
                    using (StringReader bodyReader = new StringReader(response.XmlData))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(EventNotificationAlert));
                        xmlData = (EventNotificationAlert)serializer.Deserialize(bodyReader);
                    }

                    return Ok(new PlateNumber
                    {
                        Data = xmlData,
                        Images = response.Images
                    });
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deserializing XML: {ex.InnerException}");
            }
        }
    }
}
