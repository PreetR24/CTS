using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class RulesService(
            ICapacityRuleRepository _capacityRepo,
            ISlaRepository _slaRepo,
            IAuditLogService _auditService,
            CareScheduleContext _db) : IRulesService
    {
        public CapacityRuleResponseDto CreateCapacityRule(CreateCapacityRuleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Scope))
                throw new ArgumentException("Scope is required.");

            var effectiveFrom = ParseDate(dto.EffectiveFrom, "EffectiveFrom");
            DateOnly? effectiveTo = null;
            if (!string.IsNullOrWhiteSpace(dto.EffectiveTo))
            {
                effectiveTo = ParseDate(dto.EffectiveTo, "EffectiveTo");
                if (effectiveTo <= effectiveFrom)
                    throw new ArgumentException("EffectiveTo must be after EffectiveFrom.");
            }
            var normalizedScope = dto.Scope.Trim();
            var duplicateRule = _capacityRepo.Search(normalizedScope, null).Any(r =>
                string.Equals(r.Scope, normalizedScope, StringComparison.OrdinalIgnoreCase) &&
                r.ScopeId == dto.ScopeId &&
                r.BufferMin == dto.BufferMin &&
                r.MaxApptsPerDay == dto.MaxApptsPerDay &&
                r.MaxConcurrentRooms == dto.MaxConcurrentRooms &&
                r.EffectiveFrom == effectiveFrom &&
                r.EffectiveTo == effectiveTo &&
                !string.Equals(r.Status, "Inactive", StringComparison.OrdinalIgnoreCase));
            if (duplicateRule)
                throw new ArgumentException("Duplicate capacity rule already exists.");

            var entity = new CapacityRule
            {
                Scope = normalizedScope,
                ScopeId = dto.ScopeId,
                MaxApptsPerDay = dto.MaxApptsPerDay,
                MaxConcurrentRooms = dto.MaxConcurrentRooms,
                BufferMin = dto.BufferMin,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                Status = "Active"
            };

            _capacityRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CREATE",
                Resource = "CapacityRule",
                Metadata = $"{{\"ruleId\":{entity.RuleId},\"scope\":\"{entity.Scope}\"}}"
            });

            _db.SaveChanges();
            return MapRule(entity);
        }

        public CapacityRuleResponseDto UpdateCapacityRule(int ruleId, UpdateCapacityRuleDto dto)
        {
            var entity = _capacityRepo.GetById(ruleId);
            if (entity == null) throw new KeyNotFoundException("Capacity rule not found.");

            if (!string.IsNullOrWhiteSpace(dto.Scope)) entity.Scope = dto.Scope.Trim();
            if (dto.ScopeId.HasValue) entity.ScopeId = dto.ScopeId;
            if (dto.MaxApptsPerDay.HasValue) entity.MaxApptsPerDay = dto.MaxApptsPerDay;
            if (dto.MaxConcurrentRooms.HasValue) entity.MaxConcurrentRooms = dto.MaxConcurrentRooms;
            if (dto.BufferMin.HasValue) entity.BufferMin = dto.BufferMin.Value;

            if (!string.IsNullOrWhiteSpace(dto.EffectiveFrom))
                entity.EffectiveFrom = ParseDate(dto.EffectiveFrom, "EffectiveFrom");

            if (!string.IsNullOrWhiteSpace(dto.EffectiveTo))
                entity.EffectiveTo = ParseDate(dto.EffectiveTo, "EffectiveTo");

            if (entity.EffectiveTo.HasValue && entity.EffectiveTo <= entity.EffectiveFrom)
                throw new ArgumentException("EffectiveTo must be after EffectiveFrom.");

            if (!string.IsNullOrWhiteSpace(dto.Status)) entity.Status = dto.Status.Trim();

            var duplicateRule = _capacityRepo.Search(entity.Scope, null).Any(r =>
                r.RuleId != entity.RuleId &&
                string.Equals(r.Scope, entity.Scope, StringComparison.OrdinalIgnoreCase) &&
                r.ScopeId == entity.ScopeId &&
                r.BufferMin == entity.BufferMin &&
                r.MaxApptsPerDay == entity.MaxApptsPerDay &&
                r.MaxConcurrentRooms == entity.MaxConcurrentRooms &&
                r.EffectiveFrom == entity.EffectiveFrom &&
                r.EffectiveTo == entity.EffectiveTo &&
                !string.Equals(r.Status, "Inactive", StringComparison.OrdinalIgnoreCase));
            if (duplicateRule)
                throw new ArgumentException("Duplicate capacity rule already exists.");

            _capacityRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "UPDATE",
                Resource = "CapacityRule",
                Metadata = $"{{\"ruleId\":{entity.RuleId}}}"
            });

            _db.SaveChanges();
            return MapRule(entity);
        }

        public CapacityRuleResponseDto GetCapacityRule(int ruleId)
        {
            var entity = _capacityRepo.GetById(ruleId);
            if (entity == null) throw new KeyNotFoundException("Capacity rule not found.");
            return MapRule(entity);
        }

        public IEnumerable<CapacityRuleResponseDto> SearchCapacityRules(string? scope, string? status)
        {
            var items = _capacityRepo.Search(scope, status);
            return items.Select(MapRule).ToList();
        }

        public void DeactivateCapacityRule(int ruleId)
        {
            var entity = _capacityRepo.GetById(ruleId);
            if (entity == null) throw new KeyNotFoundException("Capacity rule not found.");

            if (entity.Status != "Inactive")
            {
                entity.Status = "Inactive";
                _capacityRepo.Update(entity);

                _auditService.CreateAudit(new AuditLogCreateDto
                {
                    Action = "DEACTIVATE",
                    Resource = "CapacityRule",
                    Metadata = $"{{\"ruleId\":{entity.RuleId}}}"
                });

                _db.SaveChanges();
            }
        }

        public SlaResponseDto CreateSla(CreateSlaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Scope))
                throw new ArgumentException("Scope is required.");
            if (string.IsNullOrWhiteSpace(dto.Metric))
                throw new ArgumentException("Metric is required.");
            if (dto.TargetValue <= 0)
                throw new ArgumentException("TargetValue must be positive.");
            var normalizedScope = dto.Scope.Trim();
            var normalizedMetric = dto.Metric.Trim();
            var normalizedUnit = string.IsNullOrWhiteSpace(dto.Unit) ? "Minutes" : dto.Unit.Trim();
            var duplicateSla = _slaRepo.Search(normalizedScope, null).Any(s =>
                string.Equals(s.Scope, normalizedScope, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(s.Metric, normalizedMetric, StringComparison.OrdinalIgnoreCase) &&
                s.TargetValue == dto.TargetValue &&
                string.Equals(s.Unit, normalizedUnit, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(s.Status, "Inactive", StringComparison.OrdinalIgnoreCase));
            if (duplicateSla)
                throw new ArgumentException("Duplicate SLA rule already exists.");

            var entity = new Sla
            {
                Scope = normalizedScope,
                Metric = normalizedMetric,
                TargetValue = dto.TargetValue,
                Unit = normalizedUnit,
                Status = "Active"
            };

            _slaRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CREATE",
                Resource = "SLA",
                Metadata = $"{{\"slaId\":{entity.Slaid},\"metric\":\"{entity.Metric}\"}}"
            });

            _db.SaveChanges();
            return MapSla(entity);
        }

        public SlaResponseDto UpdateSla(int slaId, UpdateSlaDto dto)
        {
            var entity = _slaRepo.GetById(slaId);
            if (entity == null) throw new KeyNotFoundException("SLA not found.");

            if (!string.IsNullOrWhiteSpace(dto.Scope)) entity.Scope = dto.Scope.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Metric)) entity.Metric = dto.Metric.Trim();
            if (dto.TargetValue.HasValue)
            {
                if (dto.TargetValue.Value <= 0)
                    throw new ArgumentException("TargetValue must be positive.");
                entity.TargetValue = dto.TargetValue.Value;
            }
            if (!string.IsNullOrWhiteSpace(dto.Unit)) entity.Unit = dto.Unit.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Status)) entity.Status = dto.Status.Trim();

            var duplicateSla = _slaRepo.Search(entity.Scope, null).Any(s =>
                s.Slaid != entity.Slaid &&
                string.Equals(s.Scope, entity.Scope, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(s.Metric, entity.Metric, StringComparison.OrdinalIgnoreCase) &&
                s.TargetValue == entity.TargetValue &&
                string.Equals(s.Unit, entity.Unit, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(s.Status, "Inactive", StringComparison.OrdinalIgnoreCase));
            if (duplicateSla)
                throw new ArgumentException("Duplicate SLA rule already exists.");

            _slaRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "UPDATE",
                Resource = "SLA",
                Metadata = $"{{\"slaId\":{entity.Slaid}}}"
            });

            _db.SaveChanges();
            return MapSla(entity);
        }

        public SlaResponseDto GetSla(int slaId)
        {
            var entity = _slaRepo.GetById(slaId);
            if (entity == null) throw new KeyNotFoundException("SLA not found.");
            return MapSla(entity);
        }

        public IEnumerable<SlaResponseDto> SearchSlas(string? scope, string? status)
        {
            var items = _slaRepo.Search(scope, status);
            return items.Select(MapSla).ToList();
        }

        public void DeactivateSla(int slaId)
        {
            var entity = _slaRepo.GetById(slaId);
            if (entity == null) throw new KeyNotFoundException("SLA not found.");

            if (entity.Status != "Inactive")
            {
                entity.Status = "Inactive";
                _slaRepo.Update(entity);

                _auditService.CreateAudit(new AuditLogCreateDto
                {
                    Action = "DEACTIVATE",
                    Resource = "SLA",
                    Metadata = $"{{\"slaId\":{entity.Slaid}}}"
                });

                _db.SaveChanges();
            }
        }

        private static CapacityRuleResponseDto MapRule(CapacityRule e) => new()
        {
            RuleId = e.RuleId,
            Scope = e.Scope,
            ScopeId = e.ScopeId,
            MaxApptsPerDay = e.MaxApptsPerDay,
            MaxConcurrentRooms = e.MaxConcurrentRooms,
            BufferMin = e.BufferMin,
            EffectiveFrom = e.EffectiveFrom.ToString("yyyy-MM-dd"),
            EffectiveTo = e.EffectiveTo?.ToString("yyyy-MM-dd"),
            Status = e.Status
        };

        private static SlaResponseDto MapSla(Sla e) => new()
        {
            SlaId = e.Slaid,
            Scope = e.Scope,
            Metric = e.Metric,
            TargetValue = e.TargetValue,
            Unit = e.Unit,
            Status = e.Status
        };

        private static DateOnly ParseDate(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{fieldName} is required.");

            if (!DateOnly.TryParseExact(value.Trim(), "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                throw new ArgumentException($"Invalid {fieldName} format. Use yyyy-MM-dd.");

            return d;
        }
    }
}