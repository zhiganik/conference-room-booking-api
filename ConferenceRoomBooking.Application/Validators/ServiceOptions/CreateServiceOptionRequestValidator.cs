using ConferenceRoomBooking.Application.Dtos.ServiceOptions;
using FluentValidation;

namespace ConferenceRoomBooking.Application.Validators.ServiceOptions;

public class CreateServiceOptionRequestValidator : AbstractValidator<CreateServiceOptionRequest>
{
    public CreateServiceOptionRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Service name is required.")
            .MaximumLength(100).WithMessage("Service name must not exceed 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");
    }
}