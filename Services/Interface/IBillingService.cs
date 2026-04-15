using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IBillingService
    {
        ChargeRefResponseDto CreateCharge(CreateChargeRefDto dto);
        ChargeRefResponseDto GetByAppointment(int appointmentId);
        IEnumerable<ChargeRefResponseDto> Search(ChargeSearchDto dto);
    }
}
