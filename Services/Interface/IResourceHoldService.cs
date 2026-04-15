using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IResourceHoldService
    {
        ResourceHoldResponseDto Create(ResourceHoldCreateDto dto);
        ResourceHoldResponseDto Update(int holdId, ResourceHoldUpdateDto dto);
        ResourceHoldResponseDto GetById(int holdId);
        IEnumerable<ResourceHoldResponseDto> Search(int? siteId, string? resourceType, int? resourceId);
        void Release(int holdId);
    }
}