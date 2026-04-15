using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IAuditLogRepository
    {
        (List<AuditLog> Items, int Total) Search(
            int? userId,
            string? action,
            string? resource,
            DateTime? from,
            DateTime? to,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir);

        AuditLog? Get(int id);
        AuditLog Create(AuditLog entity);
    }
}