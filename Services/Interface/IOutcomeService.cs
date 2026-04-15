using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IOutcomeService
    {
        OutcomeResponseDto RecordOutcome(int appointmentId, RecordOutcomeRequestDto dto);
        OutcomeResponseDto MarkNoShow(int appointmentId, RecordOutcomeRequestDto dto);
    }
}
