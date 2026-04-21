using System;
using System.Collections.Generic;

namespace CareSchedule.DTOs
{
    // ----------------------------
    // Create (Book) Appointment
    // ----------------------------
    public class BookAppointmentRequestDto
    {
        public int PublishedSlotId { get; set; }
        public int PatientId { get; set; }
        public string BookingChannel { get; set; } = "FrontDesk"; // FrontDesk | Portal | CallCenter
    }

    public class AppointmentResponseDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public int SiteId { get; set; }
        public string? SiteName { get; set; }
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public DateOnly SlotDate { get; set; }
        public string StartTime { get; set; } = ""; // "HH:mm"
        public string EndTime { get; set; } = "";   // "HH:mm"
        public string Status { get; set; } = "";    // Booked | CheckedIn | Completed | Cancelled | NoShow
        public string BookingChannel { get; set; } = "";
    }

    // ----------------------------
    // Reschedule Appointment
    // ----------------------------
    public class RescheduleAppointmentRequestDto
    {
        public int NewPublishedSlotId { get; set; }
        public string? Reason { get; set; }
    }

    // ----------------------------
    // Cancel Appointment
    // ----------------------------
    public class CancelAppointmentRequestDto
    {
        public string? Reason { get; set; }
    }

    // ----------------------------
    // Query / Search
    // ----------------------------
    public class AppointmentSearchRequestDto
    {
        public int? PatientId { get; set; }
        public int? ProviderId { get; set; }
        public int? SiteId { get; set; }
        public string? Date { get; set; }   // "yyyy-MM-dd"
        public string? Status { get; set; } // optional filter
    }
}