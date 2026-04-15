using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IRulesService
    {
        CapacityRuleResponseDto CreateCapacityRule(CreateCapacityRuleDto dto);
        CapacityRuleResponseDto UpdateCapacityRule(int ruleId, UpdateCapacityRuleDto dto);
        CapacityRuleResponseDto GetCapacityRule(int ruleId);
        IEnumerable<CapacityRuleResponseDto> SearchCapacityRules(string? scope, string? status);
        void DeactivateCapacityRule(int ruleId);

        SlaResponseDto CreateSla(CreateSlaDto dto);
        SlaResponseDto UpdateSla(int slaId, UpdateSlaDto dto);
        SlaResponseDto GetSla(int slaId);
        IEnumerable<SlaResponseDto> SearchSlas(string? scope, string? status);
        void DeactivateSla(int slaId);
    }
}