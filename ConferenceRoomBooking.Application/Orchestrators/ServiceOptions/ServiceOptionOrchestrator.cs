using ConferenceRoomBooking.Application.Dtos.ServiceOptions;
using ConferenceRoomBooking.Application.Exceptions;
using ConferenceRoomBooking.Application.Mappers;
using ConferenceRoomBooking.DataLayer;
using ConferenceRoomBooking.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Application.Orchestrators.ServiceOptions;

public class ServiceOptionOrchestrator(AppDbContext dbContext) : IServiceOptionOrchestrator
{
    public async Task<ServiceOptionResponse> CreateAsync(CreateServiceOptionRequest request, CancellationToken cancellationToken)
    {
        await EnsureNameIsUniqueAsync(request.Name, null, cancellationToken);

        var serviceOption = new ServiceOption
        {
            Name = request.Name,
            Price = request.Price
        };

        dbContext.ServiceOptions.Add(serviceOption);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(serviceOption.Id, cancellationToken);
    }

    public async Task<ServiceOptionResponse> GetByIdAsync(int serviceOptionId, CancellationToken cancellationToken)
    {
        var result = await dbContext.ServiceOptions
            .Where(s => s.Id == serviceOptionId)
            .ToResponse()
            .FirstOrDefaultAsync(cancellationToken);
        
        return result ?? throw new NotFoundException(nameof(ServiceOption), serviceOptionId);
    }

    public async Task<ServiceOptionResponse> UpdateAsync(int serviceOptionId, UpdateServiceOptionRequest request, CancellationToken cancellationToken)
    {
        var serviceOption = await dbContext.ServiceOptions
            .FirstOrDefaultAsync(s => s.Id == serviceOptionId, cancellationToken);

        if (serviceOption is null)
        {
            throw new NotFoundException(nameof(ServiceOption), serviceOptionId);
        }

        await EnsureNameIsUniqueAsync(request.Name, serviceOptionId, cancellationToken);

        serviceOption.Name = request.Name;
        serviceOption.Price = request.Price;

        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(serviceOptionId, cancellationToken);
    }

    public async Task DeleteAsync(int serviceOptionId, CancellationToken cancellationToken)
    {
        var serviceOption = await dbContext.ServiceOptions
            .FirstOrDefaultAsync(s => s.Id == serviceOptionId, cancellationToken);

        if (serviceOption is null)
        {
            throw new NotFoundException(nameof(ServiceOption), serviceOptionId);
        }
        
        var isInUse = await dbContext.RoomServiceOptions
            .AnyAsync(rso => rso.ServiceOptionId == serviceOptionId, cancellationToken);

        if (isInUse)
        {
            throw new ConflictException(
                $"ServiceOption '{serviceOption.Name}' is linked to one or more rooms and cannot be deleted.");
        }

        dbContext.ServiceOptions.Remove(serviceOption);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceOptionResponse>> SearchAsync(SearchServiceOptionsRequest request, CancellationToken cancellationToken)
    {
        var query = dbContext.ServiceOptions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(s => s.Name.Contains(request.Name));
        }

        return await query
            .OrderBy(s => s.Name)
            .ToResponse()
            .ToListAsync(cancellationToken);
    }
    
    private async Task EnsureNameIsUniqueAsync(string name, int? excludingId, CancellationToken cancellationToken)
    {
        var query = dbContext.ServiceOptions.Where(s => s.Name == name);
        
        if (excludingId.HasValue)
        {
            query = query.Where(s => s.Id != excludingId.Value);
        }
        
        if (await query.AnyAsync(cancellationToken))
        {
            throw new ConflictException($"A service named '{name}' already exists.");
        }
    }
}