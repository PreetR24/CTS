// File: CareSchedule.Services.Implementation/AuditLogService.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class AuditLogService(IAuditLogRepository _auditrepo) : IAuditLogService
    {
        public List<AuditLogDto> SearchAudit(AuditLogSearchQuery query)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 25 : query.PageSize;

            var fromUtc = ParseIsoInstant(query.From)?.UtcDateTime;
            var toUtc   = ParseIsoInstant(query.To)?.UtcDateTime;

            var (items, _) = _auditrepo.Search(
                userId:   query.UserId,
                action:   query.Action,
                resource: query.Resource,
                from:     fromUtc,
                to:       toUtc,
                page:     page,
                pageSize: pageSize,
                sortBy:   string.IsNullOrWhiteSpace(query.SortBy) ? "timestamp" : query.SortBy,
                sortDir:  string.IsNullOrWhiteSpace(query.SortDir) ? "desc" : query.SortDir
            );

            var list = new List<AuditLogDto>(items.Count);
            foreach (var a in items) list.Add(Map(a));
            return list;
        }

        public AuditLogDto GetAudit(int id)
        {
            var e = _auditrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("Audit log not found.");
            return Map(e);
        }

        public AuditLogDto CreateAudit(AuditLogCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Action))
                throw new ArgumentException("Action is required.");
            if (string.IsNullOrWhiteSpace(dto.Resource))
                throw new ArgumentException("Resource is required.");

            var instant = ParseIsoInstant(dto.Timestamp) ?? DateTimeOffset.UtcNow;

            var e = new AuditLog
            {
                UserId    = dto.UserId,
                Action    = dto.Action.Trim(),
                Resource  = dto.Resource.Trim(),
                Timestamp = instant.UtcDateTime, // store UTC in DB
                Metadata  = dto.Metadata
            };

            e = _auditrepo.Create(e);
            return Map(e);
        }

        // ---- helpers ----

        private static AuditLogDto Map(AuditLog a) => new()
        {
            AuditId   = a.AuditId,
            UserId    = a.UserId,
            Action    = a.Action,
            Resource  = a.Resource,
            // a.Timestamp assumed UTC in DB; format to ISO 8601 string.
            Timestamp = ToIso(a.Timestamp),
            Metadata  = a.Metadata
        };

        private static DateTimeOffset? ParseIsoInstant(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
                return dto;
            throw new ArgumentException("Invalid timestamp. Use ISO 8601 like 2026-03-05T10:00:00+05:30");
        }

        private static string ToIso(DateTime utc)
        {
            var v = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
            return v.ToString("o", CultureInfo.InvariantCulture);
        }
    }
}