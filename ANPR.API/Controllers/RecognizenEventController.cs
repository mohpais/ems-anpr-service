using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateRecognizenEvent([FromBody] CreateRecognizenEventCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            try
            {
                //var requestOrder = Cre
                //bool commandResult = false;
                //requestId = Guid.NewGuid().ToString();
                //command.EmpCode = "50144438";

                //if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
                //{
                //    var requestOrder = new IdentifiedCommand<CreateRecognizenEventCommand, bool>(command, guid);

                //    _logger.LogInformation(
                //        "----- Sending command: {CommandName} ({@Command})",
                //        requestOrder.GetGenericTypeName(),
                //        requestOrder);

                var commandResult = await _mediator.Send(command);

                if (!commandResult)
                    return BadRequest();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
