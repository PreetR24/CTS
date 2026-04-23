using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IOutcomeService
    {
        OutcomeResponseDto? GetOutcomeByAppointment(int appointmentId);
        OutcomeResponseDto RecordOutcome(int appointmentId, RecordOutcomeRequestDto dto);
    }
}
