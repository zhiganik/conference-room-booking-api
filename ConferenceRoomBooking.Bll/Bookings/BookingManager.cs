using ConferenceRoomBooking.Bll.Common.Bookings;
using ConferenceRoomBooking.Bll.Common.Bookings.Models;
using ConferenceRoomBooking.Bll.Common.Rooms;
using ConferenceRoomBooking.Bll.Common.Rooms.Models;
using ConferenceRoomBooking.Bll.Common.ServiceOptions;
using ConferenceRoomBooking.Bll.Common.ServiceOptions.Models;
using ConferenceRoomBooking.Bll.Common.Shared.Abstractions;
using ConferenceRoomBooking.Bll.Common.Shared.Exceptions;

namespace ConferenceRoomBooking.Bll.Bookings;

public class BookingManager(
    IBookingRepository bookingRepository,
    IRoomRepository roomRepository,
    IServiceOptionRepository serviceOptionRepository,
    IRentalPriceCalculator priceCalculator,
    IUserContext userContext,
    IRoomBookingLock roomBookingLock) : IBookingManager
{
    public async Task<Booking> CreateAsync(Guid roomId, DateTime startTime, int durationMinutes, List<Guid>? serviceOptionIds, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();

        var room = await roomRepository.GetByIdAsync(roomId, cancellationToken)
            ?? throw new NotFoundException(nameof(Room), roomId);

        var endTime = startTime.AddMinutes(durationMinutes);
        var ids = serviceOptionIds ?? [];

        var selectedServices = await ResolveServiceOptionsAsync(ids, cancellationToken);

        var unavailableIds = ids.Except(room.Services.Select(s => s.Id));
        if (unavailableIds.Count() > 0)
        {
            throw new ConflictException(
                $"Service option(s) {string.Join(", ", unavailableIds)} are not offered by room '{room.Name}'.");
        }

        var priceBreakdown = priceCalculator.Calculate(room.BaseHourRate, startTime, endTime,
            selectedServices.Select(s => s.Price));

        Guid bookingId;
        using (await roomBookingLock.AcquireAsync(roomId, cancellationToken))
        {
            var booking = new Booking
            {
                RoomId = room.Id,
                RoomName = room.Name,
                UserId = currentUserId,
                StartTime = startTime,
                EndTime = endTime,
                BaseRoomCost = priceBreakdown.BaseRoomCost,
                ServicesCost = priceBreakdown.ServicesCost,
                TotalPrice = priceBreakdown.TotalPrice,
                CreatedAtUtc = DateTime.UtcNow,
                Services = selectedServices
                    .Select(s => new BookedServiceOption { ServiceOptionId = s.Id, Name = s.Name, PriceAtBooking = s.Price })
                    .ToList()
            };

            bookingId = await bookingRepository.CreateAsync(booking, cancellationToken);
        }

        return await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
            ?? throw new NotFoundException(nameof(Booking), bookingId);
    }

    public async Task<Booking> GetByIdAsync(Guid bookingId, CancellationToken cancellationToken) =>
        await bookingRepository.GetByIdAsync(bookingId, cancellationToken)
        ?? throw new NotFoundException(nameof(Booking), bookingId);

    public async Task<IReadOnlyList<Booking>> GetByUserAsync(CancellationToken cancellationToken) =>
        await bookingRepository.GetByUserIdAsync(GetCurrentUserId(), cancellationToken);

    private Guid GetCurrentUserId() =>
        userContext.UserId is { } id && Guid.TryParse(id, out var userId)
            ? userId
            : throw new UnauthorizedException("User is not authenticated.");

    private async Task<IEnumerable<ServiceOption>> ResolveServiceOptionsAsync(IReadOnlyCollection<Guid> serviceOptionIds, CancellationToken cancellationToken)
    {
        if (serviceOptionIds.Count == 0)
        {
            return [];
        }

        var existing = await serviceOptionRepository.GetByIdsAsync(serviceOptionIds, cancellationToken);
        if (existing.Count != serviceOptionIds.Count)
        {
            var missingIds = serviceOptionIds.Except(existing.Select(s => s.Id));
            throw new NotFoundException(nameof(ServiceOption), string.Join(", ", missingIds));
        }

        return existing;
    }
}
