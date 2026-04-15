using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class ReportService(IOpsReportRepository _reportRepo, IUnitOfWork _uow) : IReportService
    {
        public OpsReportResponseDto Create(CreateReportDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Scope))
                throw new ArgumentException("Scope is required.");

            var entity = new OpsReport
            {
                Scope = dto.Scope.Trim(),
                MetricsJson = dto.MetricsJson,
                GeneratedDate = DateTime.UtcNow
            };
            _reportRepo.Add(entity);
            _uow.SaveChanges();
            return Map(entity);
        }

        public OpsReportResponseDto GetById(int reportId)
        {
            if (reportId <= 0) throw new ArgumentException("Invalid reportId.");
            var entity = _reportRepo.GetById(reportId);
            if (entity == null) throw new KeyNotFoundException("Report not found.");
            return Map(entity);
        }

        public IEnumerable<OpsReportResponseDto> Search(ReportSearchDto dto)
        {
            var from = ParseDateNullable(dto.FromDate);
            var to = ParseDateNullable(dto.ToDate);
            return _reportRepo.Search(dto.Scope, from, to).Select(Map).ToList();
        }

        public byte[] Export(ReportSearchDto dto)
        {
            var rows = Search(dto).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("ReportId,Scope,GeneratedDate,MetricsJson");
            foreach (var r in rows)
            {
                var metrics = (r.MetricsJson ?? string.Empty).Replace("\"", "\"\"");
                sb.AppendLine($"{r.ReportId},{r.Scope},{r.GeneratedDate:yyyy-MM-ddTHH:mm:ssZ},\"{metrics}\"");
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private static DateTime? ParseDateNullable(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (!DateTime.TryParseExact(value.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                throw new ArgumentException("Invalid date format. Use yyyy-MM-dd.");
            return parsed;
        }

        private static OpsReportResponseDto Map(OpsReport r) => new()
        {
            ReportId = r.ReportId,
            Scope = r.Scope,
            MetricsJson = r.MetricsJson,
            GeneratedDate = r.GeneratedDate
        };
    }
}
