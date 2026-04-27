using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IBookingService
    {
        AppointmentResponseDto Book(BookAppointmentRequestDto dto);
        AppointmentResponseDto Reschedule(int appointmentId, RescheduleAppointmentRequestDto dto, string requesterRole);
        void Cancel(int appointmentId, CancelAppointmentRequestDto dto);
        AppointmentResponseDto GetById(int appointmentId);
        IEnumerable<AppointmentResponseDto> Search(AppointmentSearchRequestDto dto);
        void MarkCheckedIn(int appointmentId);
        void MarkComplete(int appointmentId);
        void MarkNoShow(int appointmentId);
        void CancelByProvider(int appointmentId);
    }
}