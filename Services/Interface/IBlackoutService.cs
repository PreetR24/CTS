using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IBlackoutService
    {
        BlackoutResponseDto Create(CreateBlackoutRequestDto dto);
        void Cancel(int blackoutId);
        void Activate(int blackoutId);
        IEnumerable<BlackoutResponseDto> List(int siteId, string? startDate, string? endDate);
    }
}
