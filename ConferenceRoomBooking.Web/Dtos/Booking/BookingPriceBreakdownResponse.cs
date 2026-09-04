namespace ConferenceRoomBooking.Web.Dtos.Booking;

public record BookingPriceBreakdownResponse(decimal BaseRoomCost, decimal TimeAdjustmentPercent, decimal AdjustedRoomCost,
    decimal ServicesCost, decimal TotalPrice);