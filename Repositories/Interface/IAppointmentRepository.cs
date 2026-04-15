using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IAppointmentRepository
    {
        void Add(Appointment entity);
        void Update(Appointment entity);
        Appointment? GetById(int appointmentId);
        IEnumerable<Appointment> Search(int? patientId, int? providerId, int? siteId, DateOnly? date, string? status);
        int CountByProviderDate(int providerId, int siteId, DateOnly date, string status);
    }
}