using System;

namespace CareSchedule.DTOs
{
    public class CreateCheckInRequestDto
    {
        public string? TokenNo { get; set; }
    }

    public class CheckInResponseDto
    {
        public int CheckInId { get; set; }
        public int AppointmentId { get; set; }
        public string? TokenNo { get; set; }
        public DateTime CheckInTime { get; set; }
        public int? RoomAssigned { get; set; }
        public string Status { get; set; } = "";
    }

    public class AssignRoomRequestDto
    {
        public int RoomId { get; set; }
    }

    public class UpdateCheckInStatusDto
    {
        public string Status { get; set; } = "";
    }

    public class CheckInSearchDto
    {
        public int? SiteId { get; set; }
        public int? ProviderId { get; set; }
        public int? NurseId { get; set; }
        public string? Status { get; set; }
    }
}
