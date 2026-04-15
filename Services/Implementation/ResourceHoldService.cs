using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class ResourceHoldService(IResourceHoldRepository _holdRepo, ICalendarEventRepository _calendarRepo, IAuditLogService _auditService) : IResourceHoldService
    {
        public ResourceHoldResponseDto Create(ResourceHoldCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ResourceType))
                throw new ArgumentException("ResourceType is required.");
            if (dto.ResourceId <= 0)
                throw new ArgumentException("ResourceId is required.");
            if (dto.SiteId <= 0)
                throw new ArgumentException("SiteId is required.");

            var start = ParseDateTime(dto.StartTime, "StartTime");
            var end = ParseDateTime(dto.EndTime, "EndTime");

            if (end <= start)
                throw new ArgumentException("EndTime must be after StartTime.");

            var entity = new ResourceHold
            {
                ResourceType = dto.ResourceType.Trim(),
                ResourceId = dto.ResourceId,
                SiteId = dto.SiteId,
                StartTime = start,
                EndTime = end,
                Reason = dto.Reason?.Trim(),
                Status = "Active"
            };

            _holdRepo.Add(entity);

            _calendarRepo.Add(new CalendarEvent
            {
                EntityType = "Hold",
                EntityId = entity.HoldId,
                ProviderId = null,
                SiteId = entity.SiteId,
                RoomId = entity.ResourceType.Equals("Room", StringComparison.OrdinalIgnoreCase) ? entity.ResourceId : null,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Status = "Active"
            });

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Create",
                Resource = "ResourceHold",
                Metadata = $"HoldId={entity.HoldId}; {entity.ResourceType} #{entity.ResourceId} at site #{entity.SiteId}"
            });

            return Map(entity);
        }

        public ResourceHoldResponseDto Update(int holdId, ResourceHoldUpdateDto dto)
        {
            var entity = _holdRepo.GetById(holdId);
            if (entity == null)
                throw new KeyNotFoundException($"ResourceHold {holdId} not found.");

            if (!string.IsNullOrWhiteSpace(dto.StartTime))
                entity.StartTime = ParseDateTime(dto.StartTime, "StartTime");
            if (!string.IsNullOrWhiteSpace(dto.EndTime))
                entity.EndTime = ParseDateTime(dto.EndTime, "EndTime");

            if (entity.EndTime <= entity.StartTime)
                throw new ArgumentException("EndTime must be after StartTime.");

            if (!string.IsNullOrWhiteSpace(dto.Reason))
                entity.Reason = dto.Reason.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Status))
                entity.Status = dto.Status.Trim();

            _holdRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Update",
                Resource = "ResourceHold",
                Metadata = $"HoldId={holdId} updated"
            });

            return Map(entity);
        }

        public ResourceHoldResponseDto GetById(int holdId)
        {
            var entity = _holdRepo.GetById(holdId);
            if (entity == null)
                throw new KeyNotFoundException($"ResourceHold {holdId} not found.");

            return Map(entity);
        }

        public IEnumerable<ResourceHoldResponseDto> Search(int? siteId, string? resourceType, int? resourceId)
        {
            var items = _holdRepo.Search(siteId, resourceType, resourceId);
            return items.Select(Map).ToList();
        }

        public void Release(int holdId)
        {
            var entity = _holdRepo.GetById(holdId);
            if (entity == null)
                throw new KeyNotFoundException($"ResourceHold {holdId} not found.");

            entity.Status = "Released";
            _holdRepo.Update(entity);

            _calendarRepo.DeleteByEntity("Hold", holdId);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "Release",
                Resource = "ResourceHold",
                Metadata = $"HoldId={holdId} released"
            });
        }

        private static ResourceHoldResponseDto Map(ResourceHold e) => new()
        {
            HoldId = e.HoldId,
            ResourceType = e.ResourceType,
            ResourceId = e.ResourceId,
            SiteId = e.SiteId,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Reason = e.Reason,
            Status = e.Status
        };

        private static DateTime ParseDateTime(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} is required.");

            if (!DateTime.TryParseExact(value.Trim(), "yyyy-MM-dd HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                throw new ArgumentException($"Invalid {fieldName} format. Use yyyy-MM-dd HH:mm.");

            return parsed;
        }
    }
}