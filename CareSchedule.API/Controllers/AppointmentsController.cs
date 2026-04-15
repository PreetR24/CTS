using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("appointments")]
    [Authorize(Roles = "FrontDesk,Provider,Patient,Admin")]
    public class AppointmentsController(IBookingService _bookingservice) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "FrontDesk,Patient,Admin")]
        public ActionResult<ApiResponse<AppointmentResponseDto>> Book([FromBody] BookAppointmentRequestDto dto)
        {
            var result = _bookingservice.Book(dto);
            return ApiResponse<AppointmentResponseDto>.Ok(result, "Appointment booked.");
        }

        [HttpPatch("{appointmentId:int}")]
        [Authorize(Roles = "FrontDesk,Patient,Admin")]
        public ActionResult<ApiResponse<AppointmentResponseDto>> Reschedule(int appointmentId, [FromBody] RescheduleAppointmentRequestDto dto)
        {
            var result = _bookingservice.Reschedule(appointmentId, dto);
            return ApiResponse<AppointmentResponseDto>.Ok(result, "Appointment rescheduled.");
        }

        [HttpPatch("{appointmentId:int}/cancel")]
        [Authorize(Roles = "FrontDesk,Patient,Admin")]
        public ActionResult<ApiResponse<object>> Cancel(int appointmentId, [FromBody] CancelAppointmentRequestDto dto)
        {
            _bookingservice.Cancel(appointmentId, dto);
            return ApiResponse<object>.Ok(null, "Appointment cancelled.");
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<AppointmentResponseDto>>> Search([FromQuery] AppointmentSearchRequestDto dto)
        {
            var list = _bookingservice.Search(dto);
            return ApiResponse<IEnumerable<AppointmentResponseDto>>.Ok(list, "Appointments fetched.");
        }

        [HttpGet("{appointmentId:int}")]
        public ActionResult<ApiResponse<AppointmentResponseDto>> GetById(int appointmentId)
        {
            var item = _bookingservice.GetById(appointmentId);
            return ApiResponse<AppointmentResponseDto>.Ok(item, "Appointment fetched.");
        }

        [HttpPatch("{appointmentId:int}/checked-in")]
        [Authorize(Roles = "FrontDesk,Admin")]
        public ActionResult<ApiResponse<object>> MarkCheckedIn(int appointmentId)
        {
            _bookingservice.MarkCheckedIn(appointmentId);
            return ApiResponse<object>.Ok(new { appointmentId }, "Appointment checked in.");
        }

        [HttpPatch("{appointmentId:int}/complete")]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<object>> MarkComplete(int appointmentId)
        {
            _bookingservice.MarkComplete(appointmentId);
            return ApiResponse<object>.Ok(new { appointmentId }, "Appointment completed.");
        }

        [HttpPatch("{appointmentId:int}/no-show")]
        [Authorize(Roles = "Provider,Admin")]
        public ActionResult<ApiResponse<object>> MarkNoShow(int appointmentId)
        {
            _bookingservice.MarkNoShow(appointmentId);
            return ApiResponse<object>.Ok(new { appointmentId }, "Appointment marked no-show.");
        }
    }
}
