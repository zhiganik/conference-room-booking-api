using ConferenceRoomBooking.Application.Dtos.ServiceOptions;

namespace ConferenceRoomBooking.Application.Dtos.Rooms;

public record  RoomResponse(
    int Id,
    string Name,
    int Capacity,
    decimal BaseHourlyRate,
    List<ServiceOptionResponse> Services);