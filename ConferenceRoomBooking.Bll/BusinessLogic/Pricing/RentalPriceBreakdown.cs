namespace ConferenceRoomBooking.Bll.BusinessLogic.Pricing;

public record RentalPriceBreakdown(decimal BaseRoomCost,
    decimal TimeAdjustmentPercent,
    decimal AdjustedRoomCost,
    decimal ServicesCost,
    decimal TotalPrice);
