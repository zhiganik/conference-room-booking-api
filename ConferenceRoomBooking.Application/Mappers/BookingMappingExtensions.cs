using ConferenceRoomBooking.Application.Dtos.Booking;
using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Application.Mappers;

public static class BookingMappingExtensions
{
    // RoomName/ServiceOptionName are snapshots taken at booking time (see BookingConfiguration /
    // BookingServiceOptionConfiguration), so this deliberately does not join Room/ServiceOption —
    // no risk of a booking silently disappearing because its room was renamed or soft-deleted.
    public static IQueryable<BookingResponse> ToResponse(this IQueryable<Booking> query) =>
        query.Select(b => new BookingResponse(
            b.Id,
            b.RoomId,
            b.RoomName,
            b.StartTime,
            b.EndTime,
            b.BookingServiceOptions
                .Select(bso => new BookedServiceOptionResponse(bso.ServiceOptionId, bso.ServiceOptionName, bso.PriceAtBooking))
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