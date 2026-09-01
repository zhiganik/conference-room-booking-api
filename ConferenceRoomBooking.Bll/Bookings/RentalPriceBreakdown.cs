namespace ConferenceRoomBooking.Bll.Bookings;

public record RentalPriceBreakdown(decimal BaseRoomCost,
    decimal TimeAdjustmentPercent,
    decimal AdjustedRoomCost,
    decimal ServicesCost,
    decimal TotalPrice);
