using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CareSchedule.Services.Implementation
{
    public class RoomService(
        IRoomRepository _roomrepo,
        ISiteRepository _siteRepo,
        IAuditLogService _auditService) : IRoomService
    {
        public List<RoomDto> SearchRoom(RoomSearchQuery q)
        {
            var page = q.Page <= 0 ? 1 : q.Page;
            var pageSize = q.PageSize <= 0 ? 25 : q.PageSize;
            var sortBy = string.IsNullOrWhiteSpace(q.SortBy) ? "roomname" : q.SortBy;
            var sortDir = string.IsNullOrWhiteSpace(q.SortDir) ? "asc" : q.SortDir;

            var (items, _) = _roomrepo.Search(
                roomName: q.RoomName,
                roomType: null,
                status: q.Status,
                siteId: q.SiteId,
                page: page,
                pageSize: pageSize,
                sortBy: sortBy,
                sortDir: sortDir
            );

            var list = new List<RoomDto>(items.Count);
            foreach (var r in items) list.Add(Map(r));
            return list;
        }

        public RoomDto GetRoom(int id)
        {
            var r = _roomrepo.Get(id);
            if (r is null) throw new KeyNotFoundException("Room not found.");
            return Map(r);
        }

        public RoomDto CreateRoom(RoomCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoomName)) throw new ArgumentException("RoomName is required.");
            if (dto.RoomName.Trim().Length < 1) throw new ArgumentException("RoomName must be at least 1 character.");
            if (!Regex.IsMatch(dto.RoomName, "[A-Za-z]")) throw new ArgumentException("RoomName must contain at least one letter.");
            if (dto.SiteId <= 0) throw new ArgumentException("SiteId is required.");
            if (!string.IsNullOrWhiteSpace(dto.RoomType) && dto.RoomType.Trim().Length < 1)
                throw new ArgumentException("RoomType must be at least 1 character.");
            if (!string.IsNullOrWhiteSpace(dto.RoomType) && !Regex.IsMatch(dto.RoomType.Trim(), @"^[A-Za-z\s]+$"))
                throw new ArgumentException("RoomType must contain only letters.");
            EnsureSiteActive(dto.SiteId);

            var e = new Room
            {
                RoomName = dto.RoomName.Trim(),
                RoomType = string.IsNullOrWhiteSpace(dto.RoomType) ? "General" : dto.RoomType.Trim(),
                SiteId = dto.SiteId,
                AttributesJson = dto.AttributesJson,
                Status = "Active"
            };
            e = _roomrepo.Create(e);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CreateRoom",
                Resource = "Room",
                Metadata = $"{{\"roomId\":{e.RoomId},\"siteId\":{e.SiteId},\"name\":\"{e.RoomName}\"}}"
            });

            return Map(e);
        }

        public RoomDto UpdateRoom(int id, RoomUpdateDto dto)
        {
            var e = _roomrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("Room not found.");

            if (dto.RoomName is not null)
            {
                if (string.IsNullOrWhiteSpace(dto.RoomName)) throw new ArgumentException("RoomName cannot be empty.");
                if (!Regex.IsMatch(dto.RoomName, "[A-Za-z]")) throw new ArgumentException("RoomName must contain at least one letter.");
                e.RoomName = dto.RoomName.Trim();
            }
            if (dto.RoomType is not null)
            {
                if (string.IsNullOrWhiteSpace(dto.RoomType)) throw new ArgumentException("RoomType cannot be empty.");
                if (!Regex.IsMatch(dto.RoomType.Trim(), @"^[A-Za-z\s]+$")) throw new ArgumentException("RoomType must contain only letters.");
                e.RoomType = dto.RoomType.Trim();
            }
            if (dto.SiteId.HasValue)
            {
                EnsureSiteActive(dto.SiteId.Value);
                e.SiteId = dto.SiteId.Value;
            }
            if (dto.AttributesJson is not null) e.AttributesJson = dto.AttributesJson;

            _roomrepo.Update(e);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "UpdateRoom",
                Resource = "Room",
                Metadata = $"{{\"roomId\":{e.RoomId}}}"
            });

            return Map(e);
        }

        public void DeactivateRoom(int id)
        {
            var e = _roomrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("Room not found.");
            if (e.Status != "Inactive")
            {
                e.Status = "Inactive";
                _roomrepo.Update(e);

                _auditService.CreateAudit(new AuditLogCreateDto
                {
                    Action = "DeactivateRoom",
                    Resource = "Room",
                    Metadata = $"{{\"roomId\":{e.RoomId}}}"
                });
            }
        }

        public void ActivateRoom(int id)
        {
            var e = _roomrepo.Get(id);
            if (e is null) throw new KeyNotFoundException("Room not found.");
            if (e.Status != "Active")
            {
                e.Status = "Active";
                _roomrepo.Update(e);

                _auditService.CreateAudit(new AuditLogCreateDto
                {
                    Action = "ActivateRoom",
                    Resource = "Room",
                    Metadata = $"{{\"roomId\":{e.RoomId}}}"
                });
            }
        }

        private static RoomDto Map(Room r) => new()
        {
            RoomId = r.RoomId,
            RoomName = r.RoomName,
            RoomType = r.RoomType,
            SiteId = r.SiteId,
            AttributesJson = r.AttributesJson,
            Status = r.Status
        };

        private void EnsureSiteActive(int siteId)
        {
            var site = _siteRepo.Get(siteId) ?? throw new KeyNotFoundException($"Site {siteId} not found.");
            if (!string.Equals(site.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Cannot use inactive site.");
        }
    }
}
