using ConferenceRoomBooking.TelegramNotifier.Web.Dtos;
using FluentValidation;

namespace ConferenceRoomBooking.TelegramNotifier.Web.Validators;

public class PushMessageRequestValidator : AbstractValidator<PushMessageRequest>
{
    public PushMessageRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(4096).WithMessage("Message must not exceed 4096 characters (Telegram's own limit).");
    }
}
