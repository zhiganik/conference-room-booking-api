using ConferenceRoomBooking.Application.Dtos.Rooms;
using FluentValidation;

namespace ConferenceRoomBooking.Application.Validators.Rooms;

public class SearchAvailableRoomsRequestValidator : AbstractValidator<SearchAvailableRoomsRequest>
{
    public SearchAvailableRoomsRequestValidator()
    {
        RuleFor(x => x.Date)
            .NotEqual(default(DateOnly)).WithMessage("Date is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Date cannot be in the past.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.");

        RuleFor(x => x)
            .Must(x => x.StartTime < x.EndTime)
            .WithMessage("StartTime must be earlier than EndTime.");
        
        RuleFor(x => x.StartTime)
            .GreaterThanOrEqualTo(new TimeOnly(6, 0))
            .WithMessage("StartTime cannot be earlier than 06:00.");

        RuleFor(x => x.EndTime)
            .LessThanOrEqualTo(new TimeOnly(23, 0))
            .WithMessage("EndTime cannot be later than 23:00.");
    }
}