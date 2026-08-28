using ConferenceRoomBooking.Application.BusinessLogic.Availability;
using ConferenceRoomBooking.Application.BusinessLogic.Pricing;
using ConferenceRoomBooking.Application.Dtos.Booking;
using ConferenceRoomBooking.Application.Exceptions;
using ConferenceRoomBooking.Application.Mappers;
using ConferenceRoomBooking.Application.Services;
using ConferenceRoomBooking.DataLayer;
using ConferenceRoomBooking.DataLayer.Entities;
using ConferenceRoomBooking.DataLayer.QueryExtensions;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Application.Orchestrators.Booking;

public class BookingOrchestrator(AppDbContext dbContext, IRentalPriceCalculator priceCalculator, 
    IRoomAvailabilityChecker availabilityChecker, IUserContext userContext) : IBookingOrchestrator
{
    public async Task<BookingResponse> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var currentUserId = userContext.UserId
            ?? throw new UnauthorizedException("User is not authenticated.");

        var room = await dbContext.Rooms
            .Include(r => r.RoomServiceOptions)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);
        
        if (room is null)
            throw new NotFoundException(nameof(Room), request.RoomId);
        
        var startTime = request.StartTime;
        var endTime = startTime.AddMinutes(request.DurationMinutes);
        
        await EnsureRoomIsAvailableAsync(request.RoomId, startTime, endTime, cancellationToken);
        
        var serviceOptionIds = request.ServiceOptionIds ?? [];
        var selectedServices = await GetServiceOptionsAsync(serviceOptionIds, cancellationToken);

        var unavailableIds = serviceOptionIds
            .Where(id => room.RoomServiceOptions.All(o => o.ServiceOptionId != id))
            .ToList();

        if (unavailableIds.Count > 0)
        {
            throw new ConflictException(
                $"Service option(s) {string.Join(", ", unavailableIds)} are not offered by room '{room.Name}'.");
        }
        
        var priceBreakdown = priceCalculator.Calculate(room.BaseHourRate, startTime, endTime, 
            selectedServices.Select(s => s.Price));

        var booking = new DataLayer.Entities.Booking
        {
            RoomId = room.Id,
            UserId = currentUserId,
            StartTime = startTime,
            EndTime = endTime,
            BaseRoomCost = priceBreakdown.BaseRoomCost,
            ServicesCost = priceBreakdown.ServicesCost,
            TotalPrice = priceBreakdown.TotalPrice,
            CreatedAtUtc = DateTime.UtcNow,
            BookingServiceOptions = selectedServices
                .Select(s => new BookingServiceOption { ServiceOptionId = s.Id, PriceAtBooking = s.Price })
                .ToList()
        };
        
        dbContext.Bookings.Add(booking);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(booking.Id, cancellationToken);
    }

    public async Task<BookingResponse> GetByIdAsync(int bookingId, CancellationToken cancellationToken)
    {
        var result = await dbContext.Bookings
            .Where(b => b.Id == bookingId)
            .ToResponse()
            .FirstOrDefaultAsync(cancellationToken);

        return result ?? throw new NotFoundException(nameof(Booking), bookingId);
    }

    public async Task<IReadOnlyList<BookingResponse>> GetByUser(CancellationToken cancellationToken)
    {
        var currentUserId = userContext.UserId
            ?? throw new UnauthorizedException("User is not authenticated.");

        return await dbContext.Bookings.Where(b => b.UserId == currentUserId)
            .ToResponse()
            .ToListAsync(cancellationToken);
    }

    private async Task EnsureRoomIsAvailableAsync(int roomId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
    {
        var todayBookings = await dbContext.Bookings
            .ForRoom(roomId)
            .Where(b => b.StartTime.Date == startTime.Date)
            .Select(b => new { b.StartTime, b.EndTime })
            .ToListAsync(cancellationToken);

        var isAvailable = availabilityChecker.IsAvailable(startTime, endTime, 
            todayBookings.Select(b => (b.StartTime, b.EndTime)));

        if (!isAvailable)
            throw new RoomUnavailableException("Room is already booked during the requested time window.");
    }
    
    private async Task<List<ServiceOption>> GetServiceOptionsAsync(List<int> serviceOptionIds, CancellationToken cancellationToken)
    {
        if (serviceOptionIds.Count == 0)
            return [];
        
        var services = await dbContext.ServiceOptions
            .Where(s => serviceOptionIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var missingIds = serviceOptionIds.Except(services.Select(s => s.Id)).ToList();
        
        if (missingIds.Count > 0)
            throw new NotFoundException(nameof(ServiceOption), string.Join(", ", missingIds));
        
        return services;
    }
}