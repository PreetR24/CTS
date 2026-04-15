using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface ILeaveService
    {
        LeaveRequestResponseDto Submit(int userId, CreateLeaveRequestDto dto);
        LeaveRequestResponseDto Cancel(int leaveId, int userId);
        LeaveRequestResponseDto GetById(int leaveId);
        IEnumerable<LeaveRequestResponseDto> Search(LeaveSearchDto dto);
        LeaveRequestResponseDto Approve(int leaveId);
        LeaveRequestResponseDto Reject(int leaveId);
        IEnumerable<LeaveImpactResponseDto> GetImpactsByLeaveId(int leaveId);
        LeaveImpactResponseDto GetImpactById(int impactId);
        LeaveImpactResponseDto CreateImpact(CreateLeaveImpactDto dto);
        LeaveImpactResponseDto ResolveImpact(int impactId, ResolveLeaveImpactDto dto);
    }
}
