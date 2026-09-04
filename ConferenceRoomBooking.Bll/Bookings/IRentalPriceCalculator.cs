namespace ConferenceRoomBooking.Bll.Bookings;

/// <summary>
/// Computes a booking's room cost, applying the time-of-day rate bands (discount/surcharge
/// multipliers on the base hourly rate), and adds the selected services' flat prices on top.
/// </summary>
public interface IRentalPriceCalculator
{
    /// <summary>
    /// Prices a single booking. A booking spanning multiple rate bands is prorated per minute
    /// against each band it overlaps and summed.
    /// </summary>
    /// <param name="baseHourlyRate">The room's base hourly rate, before any time-of-day adjustment.</param>
    /// <param name="startTime">The booking's start time.</param>
    /// <param name="endTime">The booking's end time. Must be on the same day as <paramref name="startTime"/>.</param>
    /// <param name="servicePrices">The flat price of each selected service option.</param>
    RentalPriceBreakdown Calculate(decimal baseHourlyRate, DateTime startTime, DateTime endTime,
        IEnumerable<decimal> servicePrices);
}
