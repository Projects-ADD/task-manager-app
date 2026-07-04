namespace TaskManager.Contracts.Requests
{
    public class UpdateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string AvatarBg { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UpdateUserAvatarRequest
    {
        public string Avatar { get; set; } = string.Empty;
        public string AvatarBg { get; set; } = string.Empty;
    }

    public class UpdateUserPasswordRequest
    {
        public string Password { get; set; } = string.Empty;
    }
}