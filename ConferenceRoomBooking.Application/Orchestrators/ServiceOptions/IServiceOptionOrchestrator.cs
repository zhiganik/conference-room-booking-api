using ConferenceRoomBooking.Application.Dtos.ServiceOptions;

namespace ConferenceRoomBooking.Application.Orchestrators.ServiceOptions;

public interface IServiceOptionOrchestrator
{
    Task<ServiceOptionResponse> CreateAsync(CreateServiceOptionRequest request, CancellationToken cancellationToken);
    Task<ServiceOptionResponse> GetByIdAsync(int serviceOptionId, CancellationToken cancellationToken);
    Task<ServiceOptionResponse> UpdateAsync(int serviceOptionId, UpdateServiceOptionRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int serviceOptionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceOptionResponse>> SearchAsync(SearchServiceOptionsRequest request, CancellationToken cancellationToken);
}