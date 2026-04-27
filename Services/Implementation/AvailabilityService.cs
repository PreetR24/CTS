using System;
using System.Globalization;
using System.Text.Json;
using CareSchedule.Models;
using CareSchedule.Services.Interface;
using CareSchedule.DTOs;
using CareSchedule.Repositories.Interface;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Shared.Time;

namespace CareSchedule.Services.Implementation
{
    /// <summary>
    /// Availability module service (synchronous). Owns:
    /// - AvailabilityTemplate
    /// - AvailabilityBlock
    /// - PublishedSlot (generation & maintenance; NO public write API)
    /// - CalendarEvent projection for Blocks
    /// 
    /// Rules enforced:
    /// - Append-only AuditLog on mutations
    /// - PublishedSlot created ONLY by Availability
    /// - Closed slots never reopen
    /// - Controllers remain thin; all logic here
    /// </summary>
    public class AvailabilityService(
            IAvailabilityTemplateRepository _templateRepo,
            IAvailabilityBlockRepository _blockRepo,
            IPublishedSlotRepository _slotRepo,
            ICalendarEventRepository _calendarRepo,
            IAuditLogService _auditService,
            ISiteRepository _siteRepo,
            IProviderRepository _providerRepo,
            IServiceRepository _serviceRepo,
            IProviderServiceRepository _providerServiceRepo,
            IBlackoutRepository _blackoutRepo,
            IHolidayRepository _holidayRepo,
            IAppointmentRepository _appointmentRepo,
            CareScheduleContext _db)
            : IAvailabilityService
    {
        public void EnsureSiteActive(int siteId)
        {
            if (siteId <= 0) throw new ArgumentException("Invalid SiteID.");

            var site = _siteRepo.Get(siteId);
            if (site == null) throw new KeyNotFoundException("Site not found.");
            if (!string.Equals(site.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Site is not active.");
        }

        public void EnsureProviderActive(int providerId)
        {
            if (providerId <= 0) throw new ArgumentException("Invalid ProviderID.");

            var provider = _providerRepo.GetById(providerId);
            if (provider == null) throw new KeyNotFoundException("Provider not found.");
            if (!string.Equals(provider.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Provider is not active.");
        }

        public void EnsureServiceActive(int serviceId)
        {
            if (serviceId <= 0) throw new ArgumentException("Invalid ServiceID.");

            var service = _serviceRepo.GetById(serviceId);
            if (service == null) throw new KeyNotFoundException("Service not found.");
            if (!string.Equals(service.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Service is not active.");
        }

        public void EnsureProviderOffersServiceActive(int providerId, int serviceId)
        {
            if (providerId <= 0) throw new ArgumentException("Invalid ProviderID.");
            if (serviceId  <= 0) throw new ArgumentException("Invalid ServiceID.");

            var ps = _providerServiceRepo.GetByProviderAndService(providerId, serviceId);
            if (ps == null) throw new KeyNotFoundException("Provider does not offer this service.");
            if (!string.Equals(ps.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("ProviderService mapping is not active.");
        }

        // =========================================================
        // Templates
        // =========================================================
        public int CreateTemplate(CreateAvailabilityTemplateRequestDto dto)
        {
            
            EnsureProviderActive(dto.ProviderId);
            EnsureSiteActive(dto.SiteId);
            ValidateTemplateDto(dto);
            EnsureNoTemplateOverlap(dto.ProviderId, dto.SiteId, dto.DayOfWeek, dto.StartTime, dto.EndTime, null);

            var entity = new AvailabilityTemplate
            {
                ProviderId      = dto.ProviderId,
                SiteId          = dto.SiteId,
                DayOfWeek       = (byte)dto.DayOfWeek,
                StartTime       = ParseTimeOnly(dto.StartTime),
                EndTime         = ParseTimeOnly(dto.EndTime),
                SlotDurationMin = dto.SlotDurationMin,
                Status          = dto.Status?.Trim() ?? "Active"
            };

            _templateRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "CreateAvailabilityTemplate",
                Resource  = "AvailabilityTemplate",
                Metadata  = SerializeJson(new
                {
                    entity.ProviderId,
                    entity.SiteId,
                    entity.DayOfWeek,
                    Start = entity.StartTime.ToString(@"HH\:mm"),
                    End   = entity.EndTime.ToString(@"HH\:mm"),
                    entity.SlotDurationMin,
                    entity.Status
                })
            });

            _db.SaveChanges();
            return entity.TemplateId; // populated by EF on SaveChanges
        }

        public void UpdateTemplate(UpdateAvailabilityTemplateRequestDto dto)
        {
            if (dto.TemplateId <= 0) throw new ArgumentException("Invalid TemplateID.");

            EnsureProviderActive(dto.ProviderId);
            EnsureSiteActive(dto.SiteId);

            ValidateTemplateDto(dto);
            EnsureNoTemplateOverlap(dto.ProviderId, dto.SiteId, dto.DayOfWeek, dto.StartTime, dto.EndTime, dto.TemplateId);

            var entity = _templateRepo.GetById(dto.TemplateId);
            if (entity == null) throw new KeyNotFoundException("Template not found.");

            var before = new
            {
                entity.ProviderId,
                entity.SiteId,
                entity.DayOfWeek,
                Start = entity.StartTime.ToString(@"HH\:mm"),
                End   = entity.EndTime.ToString(@"HH\:mm"),
                entity.SlotDurationMin,
                entity.Status
            };

            entity.ProviderId      = dto.ProviderId;
            entity.SiteId          = dto.SiteId;
            entity.DayOfWeek       = (byte)dto.DayOfWeek;
            entity.StartTime       = ParseTimeOnly(dto.StartTime);
            entity.EndTime         = ParseTimeOnly(dto.EndTime);
            entity.SlotDurationMin = dto.SlotDurationMin;
            entity.Status          = dto.Status?.Trim() ?? entity.Status;

            _templateRepo.Update(entity);

            var after = new
            {
                entity.ProviderId,
                entity.SiteId,
                entity.DayOfWeek,
                Start = entity.StartTime.ToString(@"HH\:mm"),
                End   = entity.EndTime.ToString(@"HH\:mm"),
                entity.SlotDurationMin,
                entity.Status
            };

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "UpdateAvailabilityTemplate",
                Resource  = "AvailabilityTemplate",
                Metadata  = SerializeJson(new
                {
                    entity.TemplateId,
                    before,
                    after
                })
            });

            _db.SaveChanges();
        }

        public IEnumerable<AvailabilityTemplateResponseDto> ListTemplates(int providerId, int siteId)
        {
            EnsureProviderActive(providerId);
            EnsureSiteActive(siteId);

            var items = _templateRepo.List(providerId, siteId);
            return items.Select(t => new AvailabilityTemplateResponseDto
            {
                TemplateId      = t.TemplateId,
                ProviderId      = t.ProviderId,
                SiteId          = t.SiteId,
                DayOfWeek       = t.DayOfWeek,
                StartTime       = t.StartTime.ToString(@"HH\:mm"),
                EndTime         = t.EndTime.ToString(@"HH\:mm"),
                SlotDurationMin = t.SlotDurationMin,
                Status          = t.Status
            }).ToList();
        }

        // =========================================================
        // Blocks
        // =========================================================
        public int CreateBlock(CreateAvailabilityBlockRequestDto dto)
        {
            // ---------- Cross-module validations ----------
            EnsureProviderActive(dto.ProviderId);
            EnsureSiteActive(dto.SiteId);

            ValidateBlockDto(dto);

            var date  = ParseDateOnly(dto.Date);     // DateOnly
            var start = ParseTimeOnly(dto.StartTime); // TimeOnly
            var end   = ParseTimeOnly(dto.EndTime);   // TimeOnly
            if (end <= start) throw new ArgumentException("EndTime must be after StartTime.");
            if (date < TimeZoneHelper.TodayIst())
                throw new ArgumentException("Cannot create availability block for a past date.");
            EnsureNoAppointmentOverlap(dto.ProviderId, dto.SiteId, date, start, end);

            // ---------- 1) Strict overlap rejection (no merge) ----------
            var sameDayActiveBlocks = _blockRepo
                .List(dto.ProviderId, dto.SiteId, date)
                .Where(b => string.Equals(b.Status, "Active", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Overlap if (b.Start < end) && (start < b.End)  (end-exclusive)
            var overlap = sameDayActiveBlocks.FirstOrDefault(b => b.StartTime < end && start < b.EndTime);
            if (overlap != null)
            {
                // Reject creation — let middleware map to 400 BAD_REQUEST
                var msg = $"Overlaps with existing block (ID={overlap.BlockId}) window {overlap.StartTime:HH\\:mm}-{overlap.EndTime:HH\\:mm} on {date:yyyy-MM-dd}.";
                throw new ArgumentException(msg);
            }

            // ---------- 2) Create block row ----------
            var block = new AvailabilityBlock
            {
                ProviderId = dto.ProviderId,
                SiteId     = dto.SiteId,
                Date       = date,
                StartTime  = start,
                EndTime    = end,
                Reason     = dto.Reason?.Trim() ?? string.Empty,
                Status     = dto.Status?.Trim() ?? "Active"
            };

            _blockRepo.Add(block);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "CreateAvailabilityBlock",
                Resource  = "AvailabilityBlock",
                Metadata  = SerializeJson(new
                {
                    dto.ProviderId,
                    dto.SiteId,
                    dto.Date,
                    dto.StartTime,
                    dto.EndTime,
                    Reason = block.Reason
                })
            });

            // First commit to obtain BlockID for CalendarEvent projection
            _db.SaveChanges();

            // ---------- 3) Close overlapping Open/Held slots (end-exclusive) ----------
            var overlappingSlots = _slotRepo.FindSlotsInWindow(
                block.ProviderId, block.SiteId, block.Date, start, end, "Open", "Held");

            foreach (var s in overlappingSlots)
            {
                s.Status = "Closed";   // Closed never re-opens
                _slotRepo.Update(s);
            }

            // ---------- 4) Project to CalendarEvent ----------
            var startDt = CombineUtc(block.Date, start); // UTC DateTime
            var endDt   = CombineUtc(block.Date, end);

            _calendarRepo.Add(new CalendarEvent
            {
                EntityType = "Block",
                EntityId   = block.BlockId,
                ProviderId = block.ProviderId,
                SiteId     = block.SiteId,
                RoomId     = null,
                StartTime  = startDt,
                EndTime    = endDt,
                Status     = "Active"
            });

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "CreateBlock_CloseSlots_ProjectCalendar",
                Resource  = "AvailabilityBlock",
                Metadata  = SerializeJson(new
                {
                    blockId    = block.BlockId,
                    providerId = block.ProviderId,
                    siteId     = block.SiteId,
                    date       = block.Date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                    start      = start.ToString("HH:mm"),
                    end        = end.ToString("HH:mm"),
                    closedSlots = overlappingSlots.Select(s => s.PubSlotId).ToList()
                })
            });

            _db.SaveChanges();
            return block.BlockId;
        }

        public void UpdateBlock(int blockId, CreateAvailabilityBlockRequestDto dto)
        {
            if (blockId <= 0) throw new ArgumentException("Invalid BlockID.");
            ValidateBlockDto(dto);
            EnsureProviderActive(dto.ProviderId);
            EnsureSiteActive(dto.SiteId);

            var block = _blockRepo.GetById(blockId);
            if (block == null) throw new KeyNotFoundException("Block not found.");
            if (!string.Equals(block.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only active blocks can be edited.");

            var oldDate = block.Date;
            var oldStart = block.StartTime;
            var oldEnd = block.EndTime;

            var newDate = ParseDateOnly(dto.Date);
            var newStart = ParseTimeOnly(dto.StartTime);
            var newEnd = ParseTimeOnly(dto.EndTime);
            if (newEnd <= newStart) throw new ArgumentException("EndTime must be after StartTime.");
            EnsureNoAppointmentOverlap(dto.ProviderId, dto.SiteId, newDate, newStart, newEnd);

            var overlap = _blockRepo.List(dto.ProviderId, dto.SiteId, newDate)
                .Where(b => b.BlockId != blockId && string.Equals(b.Status, "Active", StringComparison.OrdinalIgnoreCase))
                .Any(b => b.StartTime < newEnd && newStart < b.EndTime);
            if (overlap) throw new ArgumentException("Cannot update block: overlaps another active block.");

            block.ProviderId = dto.ProviderId;
            block.SiteId = dto.SiteId;
            block.Date = newDate;
            block.StartTime = newStart;
            block.EndTime = newEnd;
            block.Reason = dto.Reason?.Trim() ?? string.Empty;
            _blockRepo.Update(block);

            _calendarRepo.DeleteByEntity("Block", blockId);
            _calendarRepo.Add(new CalendarEvent
            {
                EntityType = "Block",
                EntityId = block.BlockId,
                ProviderId = block.ProviderId,
                SiteId = block.SiteId,
                RoomId = null,
                StartTime = CombineUtc(newDate, newStart),
                EndTime = CombineUtc(newDate, newEnd),
                Status = "Active"
            });

            var oldWindowSlots = _slotRepo.FindSlotsInWindow(block.ProviderId, block.SiteId, oldDate, oldStart, oldEnd, "Closed");
            var activeBlocksForOldDate = _blockRepo.List(block.ProviderId, block.SiteId, oldDate)
                .Where(b => b.BlockId != blockId && string.Equals(b.Status, "Active", StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var s in oldWindowSlots)
            {
                var overlapsAnyRemainingBlock = activeBlocksForOldDate.Any(b => b.StartTime < s.EndTime && s.StartTime < b.EndTime);
                var overlapsUpdatedBlock = s.SlotDate == newDate && newStart < s.EndTime && s.StartTime < newEnd;
                if (!overlapsAnyRemainingBlock && !overlapsUpdatedBlock)
                {
                    s.Status = "Open";
                    _slotRepo.Update(s);
                }
            }

            var newOverlappingSlots = _slotRepo.FindSlotsInWindow(block.ProviderId, block.SiteId, newDate, newStart, newEnd, "Open", "Held");
            foreach (var s in newOverlappingSlots)
            {
                s.Status = "Closed";
                _slotRepo.Update(s);
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "UpdateAvailabilityBlock",
                Resource = "AvailabilityBlock",
                Metadata = SerializeJson(new
                {
                    blockId = block.BlockId,
                    before = new
                    {
                        date = oldDate.ToString("yyyy-MM-dd"),
                        start = oldStart.ToString(@"HH\:mm"),
                        end = oldEnd.ToString(@"HH\:mm")
                    },
                    after = new
                    {
                        date = newDate.ToString("yyyy-MM-dd"),
                        start = newStart.ToString(@"HH\:mm"),
                        end = newEnd.ToString(@"HH\:mm")
                    }
                })
            });

            _db.SaveChanges();
        }

        public void RemoveBlock(int blockId)
        {
            if (blockId <= 0) throw new ArgumentException("Invalid BlockID.");

            var block = _blockRepo.GetById(blockId);
            if (block == null) throw new KeyNotFoundException("Block not found.");
         
            EnsureProviderActive(block.ProviderId);
            EnsureSiteActive(block.SiteId);

            var prev = block.Status;
            // Soft delete (no hard deletes): mark as Cancelled
            block.Status = "Cancelled";
            _blockRepo.Update(block);

            // Clean the projection
            _calendarRepo.DeleteByEntity("Block", blockId);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "RemoveAvailabilityBlock",
                Resource  = "AvailabilityBlock",
                Metadata  = SerializeJson(new { blockId, previousStatus = prev })
            });

            _db.SaveChanges();
        }

        public void ActivateBlock(int blockId)
        {
            if (blockId <= 0) throw new ArgumentException("Invalid BlockID.");

            var block = _blockRepo.GetById(blockId);
            if (block == null) throw new KeyNotFoundException("Block not found.");

            EnsureProviderActive(block.ProviderId);
            EnsureSiteActive(block.SiteId);

            if (string.Equals(block.Status, "Active", StringComparison.OrdinalIgnoreCase))
                return;

            var sameDayActiveBlocks = _blockRepo
                .List(block.ProviderId, block.SiteId, block.Date)
                .Where(b =>
                    b.BlockId != block.BlockId &&
                    string.Equals(b.Status, "Active", StringComparison.OrdinalIgnoreCase))
                .ToList();
            var overlaps = sameDayActiveBlocks.Any(b => b.StartTime < block.EndTime && block.StartTime < b.EndTime);
            if (overlaps)
                throw new ArgumentException("Cannot activate this block because it overlaps another active block.");

            block.Status = "Active";
            _blockRepo.Update(block);

            var startDt = CombineUtc(block.Date, block.StartTime);
            var endDt = CombineUtc(block.Date, block.EndTime);
            _calendarRepo.Add(new CalendarEvent
            {
                EntityType = "Block",
                EntityId = block.BlockId,
                ProviderId = block.ProviderId,
                SiteId = block.SiteId,
                RoomId = null,
                StartTime = startDt,
                EndTime = endDt,
                Status = "Active"
            });

            var overlappingSlots = _slotRepo.FindSlotsInWindow(
                block.ProviderId, block.SiteId, block.Date, block.StartTime, block.EndTime, "Open", "Held");
            foreach (var s in overlappingSlots)
            {
                s.Status = "Closed";
                _slotRepo.Update(s);
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "ActivateAvailabilityBlock",
                Resource = "AvailabilityBlock",
                Metadata = SerializeJson(new
                {
                    blockId = block.BlockId,
                    providerId = block.ProviderId,
                    siteId = block.SiteId
                })
            });

            _db.SaveChanges();
        }

        public IEnumerable<AvailabilityBlockResponseDto> ListBlocks(int providerId, int siteId, string? date)
        {
            EnsureProviderActive(providerId);
            EnsureSiteActive(siteId);

            if (providerId <= 0 || siteId <= 0)
                throw new ArgumentException("providerId and siteId are required.");

            DateOnly? d = null;
            if (!string.IsNullOrWhiteSpace(date))
                d = ParseDateOnly(date);

            var items = _blockRepo.List(providerId, siteId, d);
            return items.Select(b => new AvailabilityBlockResponseDto
            {
                BlockId   = b.BlockId,
                ProviderId= b.ProviderId,
                SiteId    = b.SiteId,
                Date      = b.Date.ToString("yyyy-MM-dd"),
                StartTime = b.StartTime.ToString(@"HH\:mm"),
                EndTime   = b.EndTime.ToString(@"HH\:mm"),
                Reason    = b.Reason ?? string.Empty,
                Status    = b.Status
            }).ToList();
        }

        // =========================================================
        // Slots (Read-only)
        // =========================================================

        public IEnumerable<SlotResponseDto> GetOpenSlots(SlotSearchRequestDto dto)
        {
            if (dto.ProviderId <= 0 || dto.ServiceId <= 0 || dto.SiteId <= 0)
                throw new ArgumentException("InvalId inputs.");
            if (string.IsNullOrWhiteSpace(dto.Date))
                throw new ArgumentException("Date is required.");

            EnsureProviderActive(dto.ProviderId);
            EnsureSiteActive(dto.SiteId);
            EnsureServiceActive(dto.ServiceId);
            EnsureProviderOffersServiceActive(dto.ProviderId, dto.ServiceId);

            var d     = ParseDateOnly(dto.Date);
            var slots = _slotRepo.GetOpenSlots(dto.ProviderId, dto.ServiceId, dto.SiteId, d);

            return slots.Select(s => new SlotResponseDto
            {
                PubSlotId  = s.PubSlotId,
                ProviderId = s.ProviderId,
                ServiceId  = s.ServiceId,
                SiteId     = s.SiteId,
                SlotDate   = s.SlotDate.ToString("yyyy-MM-dd"),
                StartTime  = s.StartTime.ToString(@"HH\:mm"),
                EndTime    = s.EndTime.ToString(@"HH\:mm"),
                Status     = s.Status
            }).ToList();
        }

        // =========================================================
        // Blackouts
        // =========================================================
        public BlackoutResponseDto CreateBlackout(CreateBlackoutRequestDto dto)
        {
            EnsureSiteActive(dto.SiteId);

            if (string.IsNullOrWhiteSpace(dto.StartDate))
                throw new ArgumentException("StartDate is required.");
            if (string.IsNullOrWhiteSpace(dto.EndDate))
                throw new ArgumentException("EndDate is required.");

            var startDate = ParseDateOnly(dto.StartDate);
            var endDate = ParseDateOnly(dto.EndDate);
            if (endDate < startDate)
                throw new ArgumentException("EndDate must be on or after StartDate.");

            var entity = new Blackout
            {
                SiteId = dto.SiteId,
                StartDate = startDate,
                EndDate = endDate,
                Reason = dto.Reason?.Trim(),
                Status = "Active"
            };

            _blackoutRepo.Add(entity);
            _db.SaveChanges();

            var slotsToClose = _slotRepo.FindBySiteDateRange(dto.SiteId, startDate, endDate, "Open", "Held");
            foreach (var s in slotsToClose)
            {
                s.Status = "Closed";
                _slotRepo.Update(s);
            }

            for (var day = startDate; day <= endDate; day = day.AddDays(1))
            {
                var startDt = CombineUtc(day, new TimeOnly(0, 0));
                var endDt = CombineUtc(day, new TimeOnly(23, 59));

                _calendarRepo.Add(new CalendarEvent
                {
                    EntityType = "Blackout",
                    EntityId = entity.BlackoutId,
                    ProviderId = null,
                    SiteId = dto.SiteId,
                    RoomId = null,
                    StartTime = startDt,
                    EndTime = endDt,
                    Status = "Active"
                });
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CreateBlackout",
                Resource = "Blackout",
                Metadata = SerializeJson(new
                {
                    entity.BlackoutId,
                    entity.SiteId,
                    StartDate = startDate.ToString("yyyy-MM-dd"),
                    EndDate = endDate.ToString("yyyy-MM-dd"),
                    ClosedSlots = slotsToClose.Count()
                })
            });

            _db.SaveChanges();

            return MapBlackout(entity);
        }

        public void CancelBlackout(int blackoutId)
        {
            if (blackoutId <= 0) throw new ArgumentException("Invalid BlackoutID.");

            var entity = _blackoutRepo.GetById(blackoutId)
                ?? throw new KeyNotFoundException("Blackout not found.");

            if (entity.Status == "Cancelled") return;

            entity.Status = "Cancelled";
            _blackoutRepo.Update(entity);

            _calendarRepo.DeleteByEntity("Blackout", blackoutId);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CancelBlackout",
                Resource = "Blackout",
                Metadata = SerializeJson(new { blackoutId })
            });

            _db.SaveChanges();
        }

        public IEnumerable<BlackoutResponseDto> ListBlackouts(int siteId, string? startDate, string? endDate)
        {
            EnsureSiteActive(siteId);

            IEnumerable<Blackout> items;
            if (!string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate))
            {
                var sd = ParseDateOnly(startDate);
                var ed = ParseDateOnly(endDate);
                items = _blackoutRepo.ListBySiteDateRange(siteId, sd, ed);
            }
            else
            {
                items = _blackoutRepo.ListBySite(siteId);
            }

            return items.Select(MapBlackout).ToList();
        }

        private static BlackoutResponseDto MapBlackout(Blackout b) => new()
        {
            BlackoutId = b.BlackoutId,
            SiteId = b.SiteId,
            StartDate = b.StartDate.ToString("yyyy-MM-dd"),
            EndDate = b.EndDate.ToString("yyyy-MM-dd"),
            Reason = b.Reason,
            Status = b.Status
        };

        // =========================================================
        // Slot generation (MVP trigger)
        // =========================================================
        public ProviderSlotGenerationResponseDto GenerateSlotsFromTemplate(ProviderSlotGenerationRequestDto dto, int? currentProviderId, bool isAdmin)
        {
            if (dto.TemplateId <= 0) throw new ArgumentException("TemplateId is required.");
            if (dto.SiteId <= 0) throw new ArgumentException("SiteId is required.");
            if (dto.Days <= 0) throw new ArgumentException("Days must be positive.");

            EnsureSiteActive(dto.SiteId);

            var template = _templateRepo.GetById(dto.TemplateId)
                ?? throw new KeyNotFoundException("Template not found.");

            if (!string.Equals(template.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Selected template is not active.");

            if (template.SiteId != dto.SiteId)
                throw new ArgumentException("Template does not belong to the selected site.");

            if (!isAdmin)
            {
                if (!currentProviderId.HasValue)
                    throw new ArgumentException("Provider identity is missing in token.");
                if (template.ProviderId != currentProviderId.Value)
                    throw new ArgumentException("You can only generate slots from your own template.");
            }

            EnsureProviderActive(template.ProviderId);

            var candidates = BuildCandidatesFromTemplate(template, dto.Days);
            if (candidates.Count == 0)
            {
                return new ProviderSlotGenerationResponseDto
                {
                    InsertedCount = 0,
                    SkippedExistingCount = 0,
                    CancelledDueToConflict = false
                };
            }

            var conflicts = new List<SlotGenerationConflictDto>();
            foreach (var c in candidates)
            {
                var overlapping = _slotRepo.FindSlotsInWindow(
                    c.ProviderId, c.SiteId, c.SlotDate, c.StartTime, c.EndTime, "Open", "Held", "Closed");

                var conflict = overlapping.FirstOrDefault();
                if (conflict != null)
                {
                    conflicts.Add(new SlotGenerationConflictDto
                    {
                        TemplateId = dto.TemplateId,
                        ProviderId = c.ProviderId,
                        SiteId = c.SiteId,
                        Date = c.SlotDate.ToString("yyyy-MM-dd"),
                        StartTime = c.StartTime.ToString("HH:mm"),
                        EndTime = c.EndTime.ToString("HH:mm"),
                        ExistingSlotId = conflict.PubSlotId,
                        ExistingStatus = conflict.Status
                    });
                }
            }

            if (conflicts.Count > 0)
            {
                _auditService.CreateAudit(new AuditLogCreateDto
                {
                    Action = "GenerateSlotsCancelledConflict",
                    Resource = "PublishedSlot",
                    Metadata = SerializeJson(new
                    {
                        dto.TemplateId,
                        dto.SiteId,
                        dto.Days,
                        conflictCount = conflicts.Count
                    })
                });

                _db.SaveChanges();
                return new ProviderSlotGenerationResponseDto
                {
                    InsertedCount = 0,
                    SkippedExistingCount = 0,
                    CancelledDueToConflict = true,
                    Conflicts = conflicts
                };
            }

            foreach (var c in candidates)
            {
                _slotRepo.AddRange(new[]
                {
                    new PublishedSlot
                    {
                        ProviderId = c.ProviderId,
                        SiteId = c.SiteId,
                        ServiceId = c.ServiceId,
                        SlotDate = c.SlotDate,
                        StartTime = c.StartTime,
                        EndTime = c.EndTime,
                        Status = "Open"
                    }
                });
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "GenerateSlotsFromTemplate",
                Resource = "PublishedSlot",
                Metadata = SerializeJson(new
                {
                    dto.TemplateId,
                    dto.SiteId,
                    dto.Days,
                    inserted = candidates.Count
                })
            });

            _db.SaveChanges();
            return new ProviderSlotGenerationResponseDto
            {
                InsertedCount = candidates.Count,
                SkippedExistingCount = 0,
                CancelledDueToConflict = false
            };
        }

        public GenerateSlotsResponseDto GenerateSlots(GenerateSlotsRequestDto dto)
        {
            EnsureSiteActive(dto.SiteId);
            if (dto.Days   <= 0) throw new ArgumentException("Days must be positive.");

            // MVP approach:
            // - Group templates by provider for the given site
            // - For each day in horizon, expand templates into slot intervals
            // - Insert only missing slots (idempotent behavior)
            var inserted = 0;
            var skipped  = 0;

            var templates = _templateRepo.ListBySiteActive(dto.SiteId).ToList();
            var today = TimeZoneHelper.TodayIst();
            var endDate = today.AddDays(dto.Days - 1);

            var holidays = _holidayRepo.Search(dto.SiteId, null, today, endDate, "Active", 1, 9999, null, null).Items;
            var holidayDates = new HashSet<DateOnly>(holidays.Select(h => h.Date));
            var blackouts = _blackoutRepo.ListBySiteDateRange(dto.SiteId, today, endDate).ToList();

            for (int offset = 0; offset < dto.Days; offset++)
            {
                var day = today.AddDays(offset);

                if (holidayDates.Contains(day))
                    continue;

                if (blackouts.Any(b => b.StartDate <= day && day <= b.EndDate))
                    continue;

                var dow = (int)day.DayOfWeek;

                var todaysTemplates = templates.Where(t =>
                    t.DayOfWeek == (byte)dow &&
                    t.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var t in todaysTemplates)
                {
                    var activeBlocksForDay = _blockRepo.List(t.ProviderId, t.SiteId, day)
                        .Where(b => string.Equals(b.Status, "Active", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    // NEW: load active services for this provider
                    var services = _providerServiceRepo.GetActiveByProvider(t.ProviderId).ToList();
                    if (services.Count == 0)
                    {
                        // optional: audit skip reason
                        continue;
                    }

                    foreach (var ps in services)
                    {
                        var current = t.StartTime;
                        while (current < t.EndTime)
                        {
                            var next = current.AddMinutes(t.SlotDurationMin);
                            if (next > t.EndTime) break;
                            var blocked = activeBlocksForDay.Any(b => b.StartTime < next && current < b.EndTime);
                            if (blocked)
                            {
                                skipped++;
                                current = next;
                                continue;
                            }

                            // Shared time lock model:
                            // if any service already has a slot for this provider/site/time,
                            // do not create another service slot in the same window.
                            var overlaps = _slotRepo.FindSlotsInWindow(t.ProviderId, t.SiteId, day, current, next,
                                "Open", "Held", "Closed");

                            var exact = overlaps.Any(s =>
                                s.SlotDate == day &&
                                s.StartTime == current &&
                                s.EndTime == next);

                            if (!exact)
                            {
                                _slotRepo.AddRange(new[]
                                {
                                    new PublishedSlot
                                    {
                                        ProviderId = t.ProviderId,
                                        SiteId     = t.SiteId,
                                        ServiceId  = ps.ServiceId, // <<< set a valid ServiceId
                                        SlotDate   = day,
                                        StartTime  = current,
                                        EndTime    = next,
                                        Status     = "Open"
                                    }
                                });
                                inserted++;
                            }
                            else
                            {
                                skipped++;
                            }

                            current = next;
                        }
                    }
                }
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "GenerateSlots",
                Resource  = "PublishedSlot",
                Metadata  = SerializeJson(new { dto.SiteId, dto.Days, inserted, skipped })
            });

            _db.SaveChanges();

            return new GenerateSlotsResponseDto
            {
                InsertedCount         = inserted,
                SkippedExistingCount  = skipped
            };
        }

        private List<SlotCandidate> BuildCandidatesFromTemplate(AvailabilityTemplate template, int days)
        {
            var candidates = new List<SlotCandidate>();
            var today = TimeZoneHelper.TodayIst();
            var endDate = today.AddDays(days - 1);

            var services = _providerServiceRepo.GetActiveByProvider(template.ProviderId).ToList();
            if (services.Count == 0)
                throw new ArgumentException("Provider has no active service mappings.");

            var holidays = _holidayRepo.Search(template.SiteId, null, today, endDate, "Active", 1, 9999, null, null).Items;
            var holidayDates = new HashSet<DateOnly>(holidays.Select(h => h.Date));
            var blackouts = _blackoutRepo.ListBySiteDateRange(template.SiteId, today, endDate).ToList();

            for (var offset = 0; offset < days; offset++)
            {
                var day = today.AddDays(offset);
                if ((int)day.DayOfWeek != template.DayOfWeek)
                    continue;
                if (holidayDates.Contains(day))
                    continue;
                if (blackouts.Any(b => b.StartDate <= day && day <= b.EndDate))
                    continue;
                var activeBlocksForDay = _blockRepo.List(template.ProviderId, template.SiteId, day)
                    .Where(b => string.Equals(b.Status, "Active", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var ps in services)
                {
                    var current = template.StartTime;
                    while (current < template.EndTime)
                    {
                        var next = current.AddMinutes(template.SlotDurationMin);
                        if (next > template.EndTime) break;
                        var blocked = activeBlocksForDay.Any(b => b.StartTime < next && current < b.EndTime);
                        if (blocked)
                        {
                            current = next;
                            continue;
                        }

                        candidates.Add(new SlotCandidate
                        {
                            ProviderId = template.ProviderId,
                            SiteId = template.SiteId,
                            ServiceId = ps.ServiceId,
                            SlotDate = day,
                            StartTime = current,
                            EndTime = next
                        });

                        current = next;
                    }
                }
            }

            return candidates;
        }

        // =========================================================
        // Helpers
        // =========================================================
        private static void ValidateTemplateDto(CreateAvailabilityTemplateRequestDto dto)
        {
            if (dto.ProviderId<= 0)            throw new ArgumentException("InvalId ProviderID.");
            if (dto.SiteId <= 0)            throw new ArgumentException("Invalid SiteID.");
            if (dto.DayOfWeek  < 0 || dto.DayOfWeek > 6)
                                                throw new ArgumentException("DayOfWeek must be 0-6.");
            if (string.IsNullOrWhiteSpace(dto.StartTime))
                                                throw new ArgumentException("StartTime is required.");
            if (string.IsNullOrWhiteSpace(dto.EndTime))
                                                throw new ArgumentException("EndTime is required.");
            if (dto.SlotDurationMin <= 0)       throw new ArgumentException("SlotDurationMin must be positive.");

            var start = ParseTimeOnly(dto.StartTime);
            var end   = ParseTimeOnly(dto.EndTime);
            if (end <= start)                    throw new ArgumentException("EndTime must be after StartTime.");
        }

        private static DateOnly ParseDateOnly(string yyyyMMdd)
        {
            if (!DateOnly.TryParseExact(yyyyMMdd.Trim(), "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var d))
                throw new ArgumentException("Invalid date format. Use yyyy-MM-dd.");
            return d;
        }

        private static void ValidateBlockDto(CreateAvailabilityBlockRequestDto dto)
        {
            if (dto.ProviderId <= 0)             throw new ArgumentException("Invalid ProviderID.");
            if (dto.SiteId    <= 0)             throw new ArgumentException("Invalid SiteID.");
            if (string.IsNullOrWhiteSpace(dto.Date))
                                                 throw new ArgumentException("Date is required.");
            if (string.IsNullOrWhiteSpace(dto.StartTime))
                                                 throw new ArgumentException("StartTime is required.");
            if (string.IsNullOrWhiteSpace(dto.EndTime))
                                                 throw new ArgumentException("EndTime is required.");
        }
                
        private static TimeOnly ParseTimeOnly(string hhmm)
        {
            if (!TimeOnly.TryParseExact(hhmm.Trim(), "HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var t))
                throw new ArgumentException("Invalid time format. Use HH:mm.");
            return t;
        }

        private static DateTime CombineUtc(DateOnly date, TimeOnly time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0, DateTimeKind.Unspecified);
        }

        private static string SerializeJson(object obj)
        {
            // Lightweight JSON serialization for AuditLog.Metadata
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        private sealed class SlotCandidate
        {
            public int ProviderId { get; set; }
            public int SiteId { get; set; }
            public int ServiceId { get; set; }
            public DateOnly SlotDate { get; set; }
            public TimeOnly StartTime { get; set; }
            public TimeOnly EndTime { get; set; }
        }

        /// <summary>
        /// MVP helper that groups templates by Provider for a given Site.
        /// NOTE: Replace with a repository method like ListBySiteActive(siteId) for production.
        /// </summary>
        private Dictionary<int, List<AvailabilityTemplate>> GroupTemplatesByProvider(int siteId)
        {
            var buckets = new Dictionary<int, List<AvailabilityTemplate>>();

            // Heuristic (MVP): probe a reasonable providerId range. Replace with a proper repo later.
            for (int providerId = 1; providerId <= 2000; providerId++)
            {
                var list = _templateRepo.List(providerId, siteId).ToList();
                if (list.Count == 0) continue;
                buckets[providerId] = list;
            }

            return buckets;
        }

        private void EnsureNoTemplateOverlap(
            int providerId,
            int siteId,
            int dayOfWeek,
            string startTime,
            string endTime,
            int? currentTemplateId)
        {
            var start = ParseTimeOnly(startTime);
            var end = ParseTimeOnly(endTime);
            var templates = _templateRepo.List(providerId, siteId)
                .Where(t =>
                    t.DayOfWeek == dayOfWeek &&
                    string.Equals(t.Status, "Active", StringComparison.OrdinalIgnoreCase) &&
                    (!currentTemplateId.HasValue || t.TemplateId != currentTemplateId.Value))
                .ToList();

            var overlaps = templates.Any(t => t.StartTime < end && start < t.EndTime);
            if (overlaps)
                throw new ArgumentException("An active template already exists for this day and time range.");
        }

        private void EnsureNoAppointmentOverlap(int providerId, int siteId, DateOnly date, TimeOnly start, TimeOnly end)
        {
            var appointments = _appointmentRepo.Search(
                patientId: null,
                providerId: providerId,
                siteId: siteId,
                date: date,
                status: null);

            var hasOverlap = appointments.Any(a =>
                !string.Equals(a.Status, "Cancelled", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(a.Status, "NoShow", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(a.Status, "Completed", StringComparison.OrdinalIgnoreCase) &&
                a.StartTime < end &&
                start < a.EndTime);

            if (hasOverlap)
                throw new ArgumentException("Cannot block this time range because one or more appointments already exist in this slot.");
        }
    }
}