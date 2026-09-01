using FluentValidation;

namespace ConferenceRoomBooking.Application.Validators;

public static class DateTimeValidatorExtensions
{
    /// <summary>
    /// Ensures the DateTime's Kind is explicitly DateTimeKind.Utc.
    /// This is only true when the client sends an ISO-8601 value with a "Z" suffix
    /// (or "+00:00"); a bare offset normalizes to Kind.Local and no offset leaves
    /// Kind.Unspecified, so both of those are rejected here.
    /// </summary>
    public static IRuleBuilderOptions<T, DateTime> MustBeUtc<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder
            .Must(date => date.Kind == DateTimeKind.Utc)
            .WithMessage("{PropertyName} must be in UTC (send an ISO 8601 value with a 'Z' suffix, e.g. 2026-08-28T14:00:00Z).");
    }
}
