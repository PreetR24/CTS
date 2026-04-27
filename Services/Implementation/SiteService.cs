using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CareSchedule.Services.Implementation
{
    public class SiteService(
        ISiteRepository _siterepo,
        IAuditLogService _auditService,
        IAvailabilityTemplateRepository _templateRepo,
        IAppointmentRepository _appointmentRepo) : ISiteService
    {
        public List<SiteDto> SearchSite(SiteSearchQuery q)
        {
            var (items, _) = _siterepo.Search(
                q.Name,
                q.Status,
                q.Page <= 0 ? 1 : q.Page,
                q.PageSize <= 0 ? 25 : q.PageSize,
                sortBy: string.IsNullOrWhiteSpace(q.SortBy) ? "name" : q.SortBy,
                sortDir: string.IsNullOrWhiteSpace(q.SortDir) ? "asc" : q.SortDir
            );

            var list = new List<SiteDto>(items.Count);
            foreach (var s in items) list.Add(Map(s));
            return list;
        }

        public SiteDto GetSite(int id)
        {
            var s = _siterepo.Get(id);
            if (s is null) throw new KeyNotFoundException("Site not found.");
            return Map(s);
        }

        public SiteDto CreateSite(SiteCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required.");
            if (!Regex.IsMatch(dto.Name, "[A-Za-z]")) throw new ArgumentException("Site name must contain at least one letter.");
            if (string.IsNullOrWhiteSpace(dto.AddressJson)) throw new ArgumentException("Address is required.");
            var e = new Site
            {
                Name = dto.Name.Trim(),
                AddressJson = dto.AddressJson.Trim(),
                Timezone = string.IsNullOrWhiteSpace(dto.Timezone) ? "UTC" : dto.Timezone.Trim(),
                Status = "Active"
            };
            e = _siterepo.Create(e);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CreateSite",
                Resource = "Site",
                Metadata = $"{{\"siteId\":{e.SiteId},\"name\":\"{e.Name}\"}}"
            });

            return Map(e);
        }

        public SiteDto UpdateSite(int id, SiteUpdateDto dto)
        {
            var e = _siterepo.Get(id);
            if (e is null) throw new KeyNotFoundException("Site not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (!Regex.IsMatch(dto.Name, "[A-Za-z]")) throw new ArgumentException("Site name must contain at least one letter.");
                e.Name = dto.Name.Trim();
            }
            if (dto.AddressJson is not null) e.AddressJson = dto.AddressJson;
            if (!string.IsNullOrWhiteSpace(dto.Timezone)) e.Timezone = dto.Timezone.Trim();

            _siterepo.Update(e);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "UpdateSite",
                Resource = "Site",
                Metadata = $"{{\"siteId\":{e.SiteId}}}"
            });

            return Map(e);
        }

        public void DeactivateSite(int id)
        {
            var e = _siterepo.Get(id);
            if (e is null) throw new KeyNotFoundException("Site not found.");
            if (e.Status != "Inactive")
            {
                var activeTemplates = _templateRepo.ListBySiteActive(id);
                if (activeTemplates.Any())
                    throw new InvalidOperationException("Cannot deactivate site: active availability templates exist.");

                var upcomingAppts = _appointmentRepo.Search(null, null, id, null, "Booked");
                if (upcomingAppts.Any())
                    throw new InvalidOperationException("Cannot deactivate site: upcoming booked appointments exist.");

                e.Status = "Inactive";
                _siterepo.Update(e);

                _auditService.CreateAudit(new AuditLogCreateDto
                {
                    Action = "DeactivateSite",
                    Resource = "Site",
                    Metadata = $"{{\"siteId\":{e.SiteId}}}"
                });
            }
        }

        public void ActivateSite(int id)
        {
            var e = _siterepo.Get(id);
            if (e is null) throw new KeyNotFoundException("Site not found.");
            if (e.Status != "Active")
            {
                e.Status = "Active";
                _siterepo.Update(e);

                _auditService.CreateAudit(new AuditLogCreateDto
                {
                    Action = "ActivateSite",
                    Resource = "Site",
                    Metadata = $"{{\"siteId\":{e.SiteId}}}"
                });
            }
        }

        private static SiteDto Map(Site s) => new()
        {
            SiteId = s.SiteId,
            Name = s.Name,
            AddressJson = s.AddressJson,
            Timezone = s.Timezone,
            Status = s.Status
        };
    }
}
