using ConferenceRoomBooking.Application.Dtos.ServiceOptions;
using ConferenceRoomBooking.DataLayer.Entities;

namespace ConferenceRoomBooking.Application.Mappers;

public static class ServiceOptionMappingExtensions
{
    public static IQueryable<ServiceOptionResponse> ToResponse(this IQueryable<ServiceOption> query) =>
        query.Select(s => new ServiceOptionResponse(s.Id, s.Name, s.Price));
}