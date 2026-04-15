using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IAuditLogService
    {
        List<AuditLogDto> SearchAudit(AuditLogSearchQuery query);
        AuditLogDto GetAudit(int id);
        AuditLogDto CreateAudit(AuditLogCreateDto dto);
    }
}