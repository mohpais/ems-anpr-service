using MediatR;
using Microsoft.Lonsum.Services.ANPR.Application.Common.SeedWork;
using Microsoft.Lonsum.Services.ANPR.Application.Repositories;
using Microsoft.Lonsum.Services.ANPR.Domain.Entities;

namespace Microsoft.Lonsum.Services.ANPR.Application.Commands
{
    public class CreateRecognizenEventCommandHandler
        : IRequestHandler<CreateRecognizenEventCommand, bool>
    {
        private readonly IRecognizenEventRepository _repository;
        public CreateRecognizenEventCommandHandler(IRecognizenEventRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(
            CreateRecognizenEventCommand command, CancellationToken cancellationToken)
        {
            var recognize = new RecognizenEvent("ANPR", command.OriginalLicensePlate, command.PlateNumber, "", command.VehicleType, "", "", "", "Dolok", command.EmpCode, "Devin", command.CaptureDate);
            _repository.Add(recognize);
            return await _repository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);
        }
    }
}
