using FluentValidation;
using Microsoft.Lonsum.Services.ANPR.Application.Commands;

namespace Microsoft.Lonsum.Services.ANPR.Application.Validations.RecognizenEvents
{
    public class CreateRecognizenEventCommandValidator : AbstractValidator<CreateRecognizenEventCommand>
    {
        public CreateRecognizenEventCommandValidator()
        {
            RuleFor(x => x.PlateNumber).NotEmpty().MinimumLength(3).MaximumLength(10);
        }
    }
}
