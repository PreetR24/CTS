using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("charges")]
    [Authorize(Roles = "FrontDesk,Admin")]
    public class ChargesController(IBillingService _billingservice) : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<ChargeRefResponseDto>>> Search([FromQuery] ChargeSearchDto dto)
        {
            var items = _billingservice.Search(dto);
            return ApiResponse<IEnumerable<ChargeRefResponseDto>>.Ok(items, "Charges fetched.");
        }

        [HttpGet("appointment/{appointmentId:int}")]
        public ActionResult<ApiResponse<ChargeRefResponseDto>> GetByAppointment(int appointmentId)
        {
            var item = _billingservice.GetByAppointment(appointmentId);
            return ApiResponse<ChargeRefResponseDto>.Ok(item, "Charge fetched.");
        }

        [HttpPost]
        public ActionResult<ApiResponse<ChargeRefResponseDto>> Create([FromBody] CreateChargeRefDto dto)
        {
            var result = _billingservice.CreateCharge(dto);
            return ApiResponse<ChargeRefResponseDto>.Ok(result, "Charge created.");
        }
    }
}
