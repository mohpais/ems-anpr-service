using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Lonsum.Services.ANPR.API.ViewModels;
using Microsoft.Lonsum.Services.ANPR.Application.Commands;
using Microsoft.Lonsum.Services.ANPR.Application.Repositories;
using System.Net;

namespace ANPR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecognizenEventController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRecognizenEventRepository _repository;

        public RecognizenEventController(
            IMediator mediator,
            IRecognizenEventRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        [Route("List")]
        [HttpGet]
        public async Task<IActionResult> GetListRecognizenEventAsync()
        {
            var response = await _repository.GetAll();

            return Ok(response);
        }

        [Route("Create")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateRecognizenEvent([FromForm] CreateRecognizenEventDTO parameters)
        {
            try
            {
                IFormFile file = parameters.File;
                // Get the current date and time
                var currentDate = DateTime.Now;
                // Format the date to "yyyy-MM-dd" as a string
                string formattedDate = currentDate.ToString("yyyy-MM-dd");
                // Save the file to folder
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), $"Files\\{formattedDate}");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Generate a unique file name for the temporary image
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(folderPath, uniqueFileName);
                // Save the uploaded image to the temporary folder
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var requestOrder = new CreateRecognizenEventCommand(
                    parameters.OriginalLicensePlate, 
                    parameters.PlateNumber, 
                    parameters.PlateColor,
                    parameters.VehicleType,
                    parameters.VehicleColor,
                    filePath,
                    "50157587",
                    parameters.CaptureDate
                );
                var commandResult = await _mediator.Send(requestOrder);

                if (!commandResult)
                {
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                    return BadRequest();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
