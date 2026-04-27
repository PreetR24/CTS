using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IAvailabilityService
    {
        // --------- Templates ---------
        int CreateTemplate(CreateAvailabilityTemplateRequestDto dto);
        void UpdateTemplate(UpdateAvailabilityTemplateRequestDto dto);
        IEnumerable<AvailabilityTemplateResponseDto> ListTemplates(int providerId, int siteId);

        // --------- Blocks ---------
        int CreateBlock(CreateAvailabilityBlockRequestDto dto);
        void UpdateBlock(int blockId, CreateAvailabilityBlockRequestDto dto);
        void RemoveBlock(int blockId);
        void ActivateBlock(int blockId);
        IEnumerable<AvailabilityBlockResponseDto> ListBlocks(int providerId, int siteId, string? date);

        // --------- Slots (Read-only) ---------
        IEnumerable<SlotResponseDto> GetOpenSlots(SlotSearchRequestDto dto);

        // --------- Blackouts ---------
        BlackoutResponseDto CreateBlackout(CreateBlackoutRequestDto dto);
        void CancelBlackout(int blackoutId);
        IEnumerable<BlackoutResponseDto> ListBlackouts(int siteId, string? startDate, string? endDate);

        // --------- Slot generation (internal trigger for MVP) ---------
        GenerateSlotsResponseDto GenerateSlots(GenerateSlotsRequestDto dto);
        ProviderSlotGenerationResponseDto GenerateSlotsFromTemplate(ProviderSlotGenerationRequestDto dto, int? currentProviderId, bool isAdmin);
    }
}