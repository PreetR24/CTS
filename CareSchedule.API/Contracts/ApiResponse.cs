using CareSchedule.Shared.Time;

namespace CareSchedule.API.Contracts
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
        public string? Message { get; set; }
        public object? Error { get; set; }
        public DateTime Timestamp { get; set; } = TimeZoneHelper.NowIst();

        public static ApiResponse<T> Ok(T data, string? message = null)
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(object error, string? message = null)
            => new() { Success = false, Error = error, Message = message };
    }
}