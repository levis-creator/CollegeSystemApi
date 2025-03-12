namespace CollegeSystemApi.DTOs.Auth
{
    public class AuthResponse
    {
        public AuthResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public AuthResponse(bool success, string message, string token): this(success, message)
        {
            Token = token;
        }

        public AuthResponse(bool success, string message, string? token = null,
                            string? fullName = null, string? email = null,
                            IList<string>? roles = null): this(success, message)
        {
            Token = token;
            FullName = fullName;
            Email = email;
            Roles = roles;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public IList<string>? Roles { get; set; }
    }
}