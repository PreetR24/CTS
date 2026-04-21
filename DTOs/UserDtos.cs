namespace CareSchedule.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = "";
        public string Role { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string Status { get; set; } = "";
    }

    public class UserCreateDto
    {
        public string Name { get; set; } = "";
        public string Role { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
    }

    public class UserUpdateDto
    {
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? RequesterRole { get; set; }
    }

    public class UserSearchQuery
    {
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }

    public class PatientSignupDto
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
    }
}