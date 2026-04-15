using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IRoomService
    {
        List<RoomDto> SearchRoom(RoomSearchQuery query);
        RoomDto GetRoom(int id);
        RoomDto CreateRoom(RoomCreateDto dto);
        RoomDto UpdateRoom(int id, RoomUpdateDto dto);
        void DeactivateRoom(int id);
        void ActivateRoom(int id);
    }
}