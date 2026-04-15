using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using System.Collections.Generic;

namespace CareSchedule.Services.Implementation
{
    public class RoomService(IRoomRepository _roomrepo, IAuditLogService _auditService) : IRoomService
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
            if (dto.SiteId <= 0) throw new ArgumentException("SiteId is required.");

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

            if (!string.IsNullOrWhiteSpace(dto.RoomName)) e.RoomName = dto.RoomName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.RoomType)) e.RoomType = dto.RoomType.Trim();
            if (dto.SiteId.HasValue) e.SiteId = dto.SiteId.Value;
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
    }
}
