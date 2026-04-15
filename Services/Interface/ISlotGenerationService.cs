using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface ISlotGenerationService
    {
        ProviderSlotGenerationResponseDto GenerateFromTemplate(ProviderSlotGenerationRequestDto dto, int? currentProviderId, bool isAdmin);
    }
}
