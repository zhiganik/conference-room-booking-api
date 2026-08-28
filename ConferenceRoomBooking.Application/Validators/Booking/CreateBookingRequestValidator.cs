using ConferenceRoomBooking.Application.Dtos.Booking;
using FluentValidation;

namespace ConferenceRoomBooking.Application.Validators.Booking;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    private static readonly TimeSpan OperatingWindowStart = new(6, 0, 0);
    private static readonly TimeSpan OperatingWindowEnd = new(23, 0, 0);
    
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("RoomId cant be negative.");

        RuleFor(x => x.StartTime)
            .NotEqual(default(DateTime)).WithMessage("StartTime is required.")
            .MustBeUtc()
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("StartTime cannot be in the past.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThanOrEqualTo(60).WithMessage("DurationMinutes must be greater than 1 hour.")
            .LessThanOrEqualTo(12 * 60).WithMessage("DurationMinutes cannot exceed 12 hours in a single booking.");

        When(x => x.StartTime != default && x.DurationMinutes > 0, () =>
        {
            RuleFor(x => x)
                .Must(x => x.StartTime.TimeOfDay >= OperatingWindowStart)
                .WithMessage("Booking cannot start earlier than 06:00.");

            RuleFor(x => x)
                .Must(x =>
                {
                    var end = x.StartTime.AddMinutes(x.DurationMinutes);
                    return end.Date == x.StartTime.Date && end.TimeOfDay <= OperatingWindowEnd;
                })
                .WithMessage("Booking must end by 23:00 on the same day it starts.");
        });

        When(x => x.ServiceOptionIds is not null, () =>
        {
            RuleForEach(x => x.ServiceOptionIds)
                .GreaterThan(0).WithMessage("ServiceOptionIds must contain valid catalog IDs.");

            RuleFor(x => x.ServiceOptionIds)
                .Must(ids => ids!.Distinct().Count() == ids.Count)
                .WithMessage("ServiceOptionIds must not contain duplicates.");
        });
    }
}