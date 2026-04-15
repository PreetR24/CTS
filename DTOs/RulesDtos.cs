using System;

namespace CareSchedule.DTOs
{
    // --------- CapacityRule ---------
    public class CreateCapacityRuleDto
    {
        public string Scope { get; set; } = "";
        public int? ScopeId { get; set; }
        public int? MaxApptsPerDay { get; set; }
        public int? MaxConcurrentRooms { get; set; }
        public int BufferMin { get; set; }
        public string EffectiveFrom { get; set; } = "";
        public string? EffectiveTo { get; set; }
    }

    public class UpdateCapacityRuleDto
    {
        public string? Scope { get; set; }
        public int? ScopeId { get; set; }
        public int? MaxApptsPerDay { get; set; }
        public int? MaxConcurrentRooms { get; set; }
        public int? BufferMin { get; set; }
        public string? EffectiveFrom { get; set; }
        public string? EffectiveTo { get; set; }
        public string? Status { get; set; }
    }

    public class CapacityRuleResponseDto
    {
        public int RuleId { get; set; }
        public string Scope { get; set; } = "";
        public int? ScopeId { get; set; }
        public int? MaxApptsPerDay { get; set; }
        public int? MaxConcurrentRooms { get; set; }
        public int BufferMin { get; set; }
        public string EffectiveFrom { get; set; } = "";
        public string? EffectiveTo { get; set; }
        public string Status { get; set; } = "";
    }

    // --------- SLA ---------
    public class CreateSlaDto
    {
        public string Scope { get; set; } = "";
        public string Metric { get; set; } = "";
        public int TargetValue { get; set; }
        public string Unit { get; set; } = "";
    }

    public class UpdateSlaDto
    {
        public string? Scope { get; set; }
        public string? Metric { get; set; }
        public int? TargetValue { get; set; }
        public string? Unit { get; set; }
        public string? Status { get; set; }
    }

    public class SlaResponseDto
    {
        public int SlaId { get; set; }
        public string Scope { get; set; } = "";
        public string Metric { get; set; } = "";
        public int TargetValue { get; set; }
        public string Unit { get; set; } = "";
        public string Status { get; set; } = "";
    }
}
