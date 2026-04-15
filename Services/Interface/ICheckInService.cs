using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface ICheckInService
    {
        CheckInResponseDto CheckIn(int appointmentId, CreateCheckInRequestDto dto);
        CheckInResponseDto AssignRoom(int checkInId, AssignRoomRequestDto dto);
        CheckInResponseDto MoveToRoom(int checkInId);
        CheckInResponseDto UpdateStatus(int checkInId, UpdateCheckInStatusDto dto);
        CheckInResponseDto StartConsultation(int checkInId);
        IEnumerable<CheckInResponseDto> Search(CheckInSearchDto dto);
        CheckInResponseDto GetById(int checkInId);
    }
}
