namespace CollegeSystemApi.DTOs.Auth;

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
                        IList<string>? roles = null, string id="") : this(success, message)
    {
        
        Token = token;
        FullName = fullName;
        Email = email;
        Roles = roles;
        Id = id;
    }

    public bool Success { get; set; }
    public string Message { get; set; }
    public string? Token { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Id { get; set; }
    public IList<string>? Roles { get; set; }

    public override string ToString()
    {
        var properties = new List<string>
            {
                $"Success: {Success}",
                $"Message: {Message}"
            };

        if (Token != null)
            properties.Add($"Token: {Token}");

        if (FullName != null)
            properties.Add($"FullName: {FullName}");

        if (Email != null)
            properties.Add($"Email: {Email}");

        if (Roles != null && Roles.Any())
            properties.Add($"Roles: {string.Join(", ", Roles)}");

        return string.Join(Environment.NewLine, properties);
    }
}