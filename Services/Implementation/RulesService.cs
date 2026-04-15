using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class RulesService(
            ICapacityRuleRepository _capacityRepo,
            ISlaRepository _slaRepo,
            IAuditLogService _auditService,
            IUnitOfWork _uow) : IRulesService
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

            var entity = new CapacityRule
            {
                Scope = dto.Scope.Trim(),
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

            _uow.SaveChanges();
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

            _capacityRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "UPDATE",
                Resource = "CapacityRule",
                Metadata = $"{{\"ruleId\":{entity.RuleId}}}"
            });

            _uow.SaveChanges();
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

                _uow.SaveChanges();
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

            var entity = new Sla
            {
                Scope = dto.Scope.Trim(),
                Metric = dto.Metric.Trim(),
                TargetValue = dto.TargetValue,
                Unit = string.IsNullOrWhiteSpace(dto.Unit) ? "Minutes" : dto.Unit.Trim(),
                Status = "Active"
            };

            _slaRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CREATE",
                Resource = "SLA",
                Metadata = $"{{\"slaId\":{entity.Slaid},\"metric\":\"{entity.Metric}\"}}"
            });

            _uow.SaveChanges();
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

            _slaRepo.Update(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "UPDATE",
                Resource = "SLA",
                Metadata = $"{{\"slaId\":{entity.Slaid}}}"
            });

            _uow.SaveChanges();
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

                _uow.SaveChanges();
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