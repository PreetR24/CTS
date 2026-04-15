using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface ISiteService
    {
        List<SiteDto> SearchSite(SiteSearchQuery query);
        SiteDto GetSite(int id);
        SiteDto CreateSite(SiteCreateDto dto);
        SiteDto UpdateSite(int id, SiteUpdateDto dto);
        void DeactivateSite(int id);
        void ActivateSite(int id);
    }
}