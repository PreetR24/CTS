namespace CareSchedule.DTOs
{
    public class ProviderSlotGenerationRequestDto
    {
        public int TemplateId { get; set; }
        public int SiteId { get; set; }
        public int Days { get; set; } = 14;
    }

    public class SlotGenerationConflictDto
    {
        public int TemplateId { get; set; }
        public int ProviderId { get; set; }
        public int SiteId { get; set; }
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int ExistingSlotId { get; set; }
        public string ExistingStatus { get; set; } = string.Empty;
    }

    public class ProviderSlotGenerationResponseDto
    {
        public int InsertedCount { get; set; }
        public int SkippedExistingCount { get; set; }
        public bool CancelledDueToConflict { get; set; }
        public List<SlotGenerationConflictDto> Conflicts { get; set; } = new();
    }
}
