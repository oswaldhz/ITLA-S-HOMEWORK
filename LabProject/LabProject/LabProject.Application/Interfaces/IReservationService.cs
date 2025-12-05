using LabProject.Application.DTOs;

namespace LabProject.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationValidationResult> ValidateAsync(CreateReservaRequest request, CancellationToken cancellationToken = default);
}
