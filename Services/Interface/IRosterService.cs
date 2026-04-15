using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IRosterService
    {
        // Shift Templates
        ShiftTemplateResponseDto CreateShiftTemplate(CreateShiftTemplateDto dto);
        ShiftTemplateResponseDto UpdateShiftTemplate(int id, UpdateShiftTemplateDto dto);
        void DeleteShiftTemplate(int id);
        ShiftTemplateResponseDto GetShiftTemplate(int id);
        IEnumerable<ShiftTemplateResponseDto> SearchShiftTemplates(ShiftTemplateSearchDto dto);

        // Rosters
        RosterResponseDto CreateRoster(CreateRosterDto dto);
        RosterResponseDto UpdateRoster(int rosterId, UpdateRosterDto dto);
        void DeleteRoster(int rosterId);
        RosterResponseDto PublishRoster(int rosterId, PublishRosterDto dto);
        RosterResponseDto GetRoster(int id);
        IEnumerable<RosterResponseDto> SearchRosters(RosterSearchDto dto);

        // Assignments
        RosterAssignmentResponseDto AssignStaff(CreateRosterAssignmentDto dto);
        RosterAssignmentResponseDto SwapShift(int assignmentId, SwapAssignmentDto dto);
        void MarkAbsent(int assignmentId);
        IEnumerable<RosterAssignmentResponseDto> SearchAssignments(RosterAssignmentSearchDto dto);

        // On-Call
        OnCallResponseDto CreateOnCall(CreateOnCallDto dto);
        OnCallResponseDto UpdateOnCall(int id, UpdateOnCallDto dto);
        OnCallResponseDto GetOnCall(int id);
        IEnumerable<OnCallResponseDto> SearchOnCall(OnCallSearchDto dto);
    }
}
