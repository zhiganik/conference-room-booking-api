using ConferenceRoomBooking.Web.Dtos.Rooms;
using FluentValidation;

namespace ConferenceRoomBooking.Web.Validators.Rooms;

public class SearchAvailableRoomsRequestValidator : AbstractValidator<SearchAvailableRoomsRequest>
{
    private static readonly TimeSpan OperatingWindowStart = new(6, 0, 0);
    private static readonly TimeSpan OperatingWindowEnd = new(23, 0, 0);
    
    public SearchAvailableRoomsRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEqual(default(DateTime)).WithMessage("StartDate is required.")
            .MustBeUtc();

        RuleFor(x => x.EndDate)
            .NotEqual(default(DateTime)).WithMessage("EndDate is required.")
            .MustBeUtc();

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.");
        
        When(x => x.StartDate != default && x.EndDate != default, () =>
        {
            RuleFor(x => x)
                .Must(x => x.StartDate.Date == x.EndDate.Date)
                .WithMessage("StartDate and EndDate must be on the same day.");

            RuleFor(x => x)
                .Must(x => x.StartDate < x.EndDate)
                .WithMessage("StartDate must be earlier than EndDate.");

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("StartDate cannot be in the past.");

            RuleFor(x => x.StartDate.TimeOfDay)
                .GreaterThanOrEqualTo(OperatingWindowStart)
                .WithMessage("StartDate time cannot be earlier than 06:00.");

            RuleFor(x => x.EndDate.TimeOfDay)
                .LessThanOrEqualTo(OperatingWindowEnd)
                .WithMessage("EndDate time cannot be later than 23:00.");
        });
    }
}