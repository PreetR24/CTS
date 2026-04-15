using System;
using System.Collections.Generic;
using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IChargeRefRepository
    {
        void Add(ChargeRef entity);
        ChargeRef? GetByAppointmentId(int appointmentId);
        IEnumerable<ChargeRef> Search(int? appointmentId, int? providerId, string? status);
    }
}
