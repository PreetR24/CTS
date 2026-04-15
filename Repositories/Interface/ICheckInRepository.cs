using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface ICheckInRepository
    {
        void Add(CheckIn entity);
        void Update(CheckIn entity);
        CheckIn? GetById(int checkInId);
        CheckIn? GetByAppointmentId(int appointmentId);
        IEnumerable<CheckIn> Search(int? siteId, int? providerId, int? nurseId, string? status);
    }
}
