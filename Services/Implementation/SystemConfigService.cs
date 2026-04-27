// File: CareSchedule.Services.Implementation/SystemConfigService.cs
using System;
using System.Collections.Generic;
using System.Globalization;
using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using CareSchedule.Shared.Time;

namespace CareSchedule.Services.Implementation
{
    public class SystemConfigService(ISystemConfigRepository _systemconfigrepo) : ISystemConfigService
    {
        private static string Iso(DateTime utc)
        {
            return utc.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        public List<SystemConfigDto> Search(SystemConfigSearchQuery q)
        {
            var page = q.Page <= 0 ? 1 : q.Page;
            var pageSize = q.PageSize <= 0 ? 25 : q.PageSize;
            var sortBy = string.IsNullOrWhiteSpace(q.SortBy) ? "updateddate" : q.SortBy;
            var sortDir = string.IsNullOrWhiteSpace(q.SortDir) ? "desc" : q.SortDir;

            var (items, _) = _systemconfigrepo.Search(q.Key, q.Scope, page, pageSize, sortBy, sortDir);

            var list = new List<SystemConfigDto>(items.Count);
            foreach (var c in items) list.Add(Map(c));
            return list;
        }

        public SystemConfigDto Get(int id)
        {
            var e = _systemconfigrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("System config not found.");
            return Map(e);
        }

        public SystemConfigDto Create(SystemConfigCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Key))
                throw new ArgumentException("Key is required.");
            if (string.IsNullOrWhiteSpace(dto.Value))
                throw new ArgumentException("Value is required.");
            var normalizedKey = dto.Key.Trim();
            var normalizedScope = string.IsNullOrWhiteSpace(dto.Scope) ? "Global" : dto.Scope.Trim();
            var existing = _systemconfigrepo.Search(normalizedKey, normalizedScope, 1, 2000, "key", "asc").Items
                .Any(x => string.Equals(x.Key, normalizedKey, StringComparison.OrdinalIgnoreCase)
                       && string.Equals(x.Scope, normalizedScope, StringComparison.OrdinalIgnoreCase));
            if (existing)
                throw new ArgumentException("Duplicate system config key already exists for this scope.");

            var e = new SystemConfig
            {
                Key = normalizedKey,
                Value = dto.Value.Trim(),
                Scope = normalizedScope,
                UpdatedBy = dto.UpdatedBy,
                UpdatedDate = TimeZoneHelper.NowIst()
            };

            e = _systemconfigrepo.Create(e);
            return Map(e);
        }

        public SystemConfigDto Update(int id, SystemConfigUpdateDto dto)
        {
            var e = _systemconfigrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("System config not found.");

            if (dto.Value is not null) e.Value = dto.Value.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Scope)) e.Scope = dto.Scope.Trim();
            if (dto.UpdatedBy.HasValue) e.UpdatedBy = dto.UpdatedBy;

            e.UpdatedDate = TimeZoneHelper.NowIst();
            _systemconfigrepo.Update(e);

            return Map(e);
        }

        public void Delete(int id)
        {
            _systemconfigrepo.Delete(id);
        }

        private static SystemConfigDto Map(SystemConfig c) => new SystemConfigDto
        {
            ConfigId = c.ConfigId,
            Key = c.Key,
            Value = c.Value,
            Scope = c.Scope,
            UpdatedBy = c.UpdatedBy,
            UpdatedDate = Iso(c.UpdatedDate)
        };
    }
}