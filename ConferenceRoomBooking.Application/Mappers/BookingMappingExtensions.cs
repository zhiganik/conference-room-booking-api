using ConferenceRoomBooking.Application.Dtos.Booking;
using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Application.Mappers;

public static class BookingMappingExtensions
{
    public static IQueryable<BookingResponse> ToResponse(this IQueryable<Booking> query) =>
        query
            .IgnoreQueryFilters()
            .Select(b => new BookingResponse(
            b.Id,
            b.RoomId,
            b.Room.Name,
            b.StartTime,
            b.EndTime,
            b.BookingServiceOptions
                .Select(bso => new BookedServiceOptionResponse(bso.ServiceOptionId, bso.ServiceOption.Name, bso.PriceAtBooking))
                .ToList(),
            new BookingPriceBreakdownResponse(
                b.BaseRoomCost,
                b.BaseRoomCost == 0
                    ? 0m
                    : Math.Round((b.TotalPrice - b.ServicesCost) / b.BaseRoomCost * 100 - 100, 2),
                b.TotalPrice - b.ServicesCost,
                b.ServicesCost,
                b.TotalPrice),
            b.TotalPrice));
}