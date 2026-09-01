namespace ConferenceRoomBooking.Bll.BusinessLogic.Pricing;

public interface IRentalPriceCalculator
{
    RentalPriceBreakdown Calculate(decimal baseHourlyRate, DateTime startTime, DateTime endTime,
        IEnumerable<decimal> servicePrices);
}
