namespace ConferenceRoomBooking.Application.BusinessLogic.Pricing;

public record RentalPriceBreakdown(decimal BaseRoomCost, 
    decimal TimeAdjustmentPercent,
    decimal AdjustedRoomCost,
    decimal ServicesCost,
    decimal TotalPrice);