using ConferenceRoomBooking.Web.Dtos.Rooms;
using FluentValidation;

namespace ConferenceRoomBooking.Web.Validators.Rooms;

public class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Room name is required.")
            .MaximumLength(200).WithMessage("Room name must not exceed 200 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.")
            .LessThanOrEqualTo(100).WithMessage("Capacity must be realistic.");

        RuleFor(x => x.BaseHourRate)
            .GreaterThanOrEqualTo(0).WithMessage("Base hourly rate cannot be negative.");

        When(x => x.ServiceOptionIds is not null, () =>
        {
            RuleFor(x => x.ServiceOptionIds)
                .Must(ids => ids!.Distinct().Count() == ids!.Count)
                .WithMessage("ServiceOptionIds must not contain duplicates.");
        });
    }
}