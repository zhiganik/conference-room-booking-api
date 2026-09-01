using ConferenceRoomBooking.Web.Dtos.ServiceOptions;

namespace ConferenceRoomBooking.Web.Dtos.Rooms;

public record  RoomResponse(
    int Id,
    string Name,
    int Capacity,
    decimal BaseHourlyRate,
    List<ServiceOptionResponse> Services);