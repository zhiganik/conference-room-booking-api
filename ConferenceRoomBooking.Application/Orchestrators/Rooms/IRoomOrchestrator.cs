using ConferenceRoomBooking.Application.Dtos.Rooms;

namespace ConferenceRoomBooking.Application.Orchestrators.Rooms;

public interface IRoomOrchestrator
{
    Task<RoomResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken);
    Task<RoomResponse> GetByIdAsync(int roomId, CancellationToken cancellationToken);
    Task<RoomResponse> UpdateAsync(int roomId, UpdateRoomRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int roomId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AvailableRoomResponse>> SearchAvailableAsync(SearchAvailableRoomsRequest request, CancellationToken cancellationToken);
}