using ConferenceRoomBooking.Application.Dtos.Rooms;
using ConferenceRoomBooking.Application.Dtos.ServiceOptions;
using ConferenceRoomBooking.DataLayer.Entities;

namespace ConferenceRoomBooking.Application.Mappers;

public static class RoomMappingExtensions
{
    public static IQueryable<RoomResponse> ToResponse(this IQueryable<Room> query) =>
        query.Select(r => new RoomResponse(
            r.Id,
            r.Name,
            r.Capacity,
            r.BaseHourRate,
            r.RoomServiceOptions
                .Select(rso => new ServiceOptionResponse(
                    rso.ServiceOption.Id,
                    rso.ServiceOption.Name,
                    rso.ServiceOption.Price))
                .ToList()));

    public static IQueryable<AvailableRoomResponse> ToAvailableResponse(this IQueryable<Room> query) =>
        query.Select(r => new AvailableRoomResponse(r.Id, r.Name, r.Capacity, r.BaseHourRate));
}