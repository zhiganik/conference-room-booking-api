using ConferenceRoomBooking.Web.Dtos.ServiceOptions;
using FluentValidation;

namespace ConferenceRoomBooking.Web.Validators.ServiceOptions;

public class SearchServiceOptionsRequestValidator : AbstractValidator<SearchServiceOptionsRequest>
{
    public SearchServiceOptionsRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name filter must not exceed 100 characters.")
            .When(x => x.Name is not null);
    }
}