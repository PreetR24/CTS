using System;

namespace CareSchedule.DTOs
{
    public class RecordOutcomeRequestDto
    {
        public string Outcome { get; set; } = "";
        public string? Notes { get; set; }
        public int? MarkedBy { get; set; }
    }

    public class OutcomeResponseDto
    {
        public int OutcomeId { get; set; }
        public int AppointmentId { get; set; }
        public string Outcome { get; set; } = "";
        public string? Notes { get; set; }
        public int? MarkedBy { get; set; }
        public DateTime MarkedDate { get; set; }
        public bool HasPrescription { get; set; }
        public string? PrescriptionFileName { get; set; }
    }
}
