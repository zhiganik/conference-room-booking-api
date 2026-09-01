namespace ConferenceRoomBooking.Application.BusinessLogic.Pricing;

public class RentalPriceCalculator : IRentalPriceCalculator
{
    private static readonly RateBand[] RateBands =
    [
        new(new TimeSpan(6, 0, 0), new TimeSpan(9, 0, 0), 0.90m),
        new(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0), 1.00m),
        new(new TimeSpan(12, 0, 0), new TimeSpan(14, 0, 0), 1.15m),
        new(new TimeSpan(14, 0, 0), new TimeSpan(18, 0, 0), 1.00m),
        new(new TimeSpan(18, 0, 0), new TimeSpan(23, 0, 0), 0.80m)
    ];
    
    public RentalPriceBreakdown Calculate(decimal baseHourlyRate, DateTime startTime, DateTime endTime, IEnumerable<decimal> servicePrices)
    {
        if (endTime <= startTime)
            throw new ArgumentException("endTime must be after startTime.", nameof(endTime));
        
        if (startTime.Date != endTime.Date)
            throw new ArgumentException("startTime and endTime must be on the same day.", nameof(endTime));
        
        var bookingStart = startTime.TimeOfDay;
        var bookingEnd = endTime.TimeOfDay;
        
        var baseRoomCost = 0m;
        var adjustedRoomCost = 0m;

        foreach (var rateBand in RateBands)
        {
            var overlapStart = Max(bookingStart, rateBand.Start);
            var overlapEnd = Min(bookingEnd, rateBand.End);
            
            if (overlapStart >= overlapEnd) continue;
            
            var overlapHours = (decimal)(overlapEnd - overlapStart).TotalHours;
            var segmentBaseCost = baseHourlyRate * overlapHours;
            baseRoomCost += segmentBaseCost;
            adjustedRoomCost += segmentBaseCost * rateBand.Multiplier;
        }

        var servicesCost = servicePrices.Sum();
        var totalPrice = adjustedRoomCost + servicesCost;
        
        var timeAdjustmentPercent = baseRoomCost == 0 
            ? 0m
            : Math.Round((adjustedRoomCost / baseRoomCost - 1) * 100, 2);
        
        return new RentalPriceBreakdown(
            Math.Round(baseRoomCost, 2),
            timeAdjustmentPercent,
            Math.Round(adjustedRoomCost, 2),
            Math.Round(servicesCost, 2),
            Math.Round(totalPrice, 2));
    }
    
    private static TimeSpan Max(TimeSpan a, TimeSpan b) => a > b ? a : b;
    private static TimeSpan Min(TimeSpan a, TimeSpan b) => a < b ? a : b;
}