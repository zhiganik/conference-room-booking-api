using ConferenceRoomBooking.Application.Dtos.ServiceOptions;

namespace ConferenceRoomBooking.Application.Dtos.Rooms;

public record  RoomResponse(
    Guid Id,
    string Name,
    int Capacity,
    decimal BaseHourlyRate,
    List<ServiceOptionResponse> Services);