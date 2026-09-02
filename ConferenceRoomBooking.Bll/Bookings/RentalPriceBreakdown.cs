namespace ConferenceRoomBooking.Bll.Bookings;

/// <summary>
/// The result of <see cref="IRentalPriceCalculator.Calculate"/>.
/// </summary>
/// <param name="BaseRoomCost">Room cost before any time-of-day rate adjustment.</param>
/// <param name="TimeAdjustmentPercent">Net effect of the rate bands, as a percentage of <paramref name="BaseRoomCost"/>.</param>
/// <param name="AdjustedRoomCost">Room cost after applying the time-of-day rate bands.</param>
/// <param name="ServicesCost">Sum of the selected services' flat prices.</param>
/// <param name="TotalPrice"><paramref name="AdjustedRoomCost"/> plus <paramref name="ServicesCost"/>.</param>
public record RentalPriceBreakdown(decimal BaseRoomCost,
    decimal TimeAdjustmentPercent,
    decimal AdjustedRoomCost,
    decimal ServicesCost,
    decimal TotalPrice);
