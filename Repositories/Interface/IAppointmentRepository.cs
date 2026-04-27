using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IAppointmentRepository
    {
        void Add(Appointment entity);
        void Update(Appointment entity);
        void AddChange(AppointmentChange entity);
        Appointment? GetById(int appointmentId);
        IEnumerable<Appointment> Search(int? patientId, int? providerId, int? siteId, DateOnly? date, string? status);
        int CountByProviderDate(int providerId, int siteId, DateOnly date, string status);
        int CountReschedulesForAppointmentOnDay(int appointmentId, DateOnly day);
        string? GetLatestRescheduleReason(int appointmentId);
    }
}