using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IReportService
    {
        OpsReportResponseDto Create(CreateReportDto dto);
        OpsReportResponseDto GetById(int reportId);
        IEnumerable<OpsReportResponseDto> Search(ReportSearchDto dto);
        byte[] Export(ReportSearchDto dto);
    }
}
