using ConferenceRoomBooking.Application.Dtos.ServiceOptions;
using FluentValidation;

namespace ConferenceRoomBooking.Application.Validators.ServiceOptions;

public class SearchServiceOptionsRequestValidator : AbstractValidator<SearchServiceOptionsRequest>
{
    public SearchServiceOptionsRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name filter must not exceed 100 characters.")
            .When(x => x.Name is not null);
    }
}