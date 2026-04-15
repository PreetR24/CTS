using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface ISystemConfigService
    {
        List<SystemConfigDto> Search(SystemConfigSearchQuery query);
        SystemConfigDto? Get(int id);
        SystemConfigDto Create(SystemConfigCreateDto dto);
        SystemConfigDto? Update(int id, SystemConfigUpdateDto dto);
        void Delete(int id);
    }
}