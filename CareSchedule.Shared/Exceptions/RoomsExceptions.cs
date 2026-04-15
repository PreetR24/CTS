namespace CareSchedule.Shared.Exceptions
{
    // Base class (optional but clean)
    public abstract class RoomException : Exception
    {
        protected RoomException(string message) : base(message) { }
    }

    // 404 - Room not found
    public sealed class RoomNotFoundException : RoomException
    {
        public RoomNotFoundException() : base("Room not found.") { }
    }

    // 404 - Site not found for Room
    public sealed class SiteNotFoundForRoomException : RoomException
    {
        public SiteNotFoundForRoomException() : base("Site not found.") { }
    }

    // 409 - Duplicate room name for this site
    public sealed class DuplicateRoomNameException : RoomException
    {
        public DuplicateRoomNameException() : base("Room name already exists for this site.") { }
    }

    // 400 - Missing RoomName
    public sealed class RoomNameRequiredException : RoomException
    {
        public RoomNameRequiredException() : base("RoomName is required.") { }
    }

    // 400 - Missing SiteId
    public sealed class SiteIdRequiredException : RoomException
    {
        public SiteIdRequiredException() : base("SiteId is required.") { }
    }

    // 400 - Invalid status
    public sealed class RoomStatusInvalidException : RoomException
    {
        public RoomStatusInvalidException() : base("Invalid room status.") { }
    }

    // 409 - Trying to activate room already active
    public sealed class RoomAlreadyActiveException : RoomException
    {
        public RoomAlreadyActiveException() : base("Room is already active.") { }
    }

    // 409 - Trying to deactivate room already inactive
    public sealed class RoomAlreadyInactiveException : RoomException
    {
        public RoomAlreadyInactiveException() : base("Room is already inactive.") { }
    }

    // 400 - Invalid JSON
    public sealed class RoomAttributesJsonInvalidException : RoomException
    {
        public RoomAttributesJsonInvalidException() : base("Invalid attributes JSON.") { }
    }

    // 409 - Cannot delete/update due to dependencies (appointments, usage, etc.)
    public sealed class RoomHasActiveDependenciesException : RoomException
    {
        public RoomHasActiveDependenciesException() : base("Room has active dependencies and cannot be modified.") { }
    }
}