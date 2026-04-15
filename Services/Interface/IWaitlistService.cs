using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IWaitlistService
    {
        WaitlistResponseDto Add(CreateWaitlistRequestDto dto);
        void Remove(int waitId);
        IEnumerable<WaitlistResponseDto> Search(WaitlistSearchDto dto);
        WaitlistResponseDto Fill(int waitId, FillWaitlistRequestDto dto);
    }
}
