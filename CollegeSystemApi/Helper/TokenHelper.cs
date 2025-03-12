using CollegeSystemApi.DTOs.Auth;
using CollegeSystemApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CollegeSystemApi.Helper;

public static class TokenHelper
{
    public static async Task<string> GenerateToken(AppUser user, UserManager<AppUser> userManager, JwtSettings jwtSettings)
    {
        var claims = new List<Claim> { 
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };
        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer:jwtSettings.Issuer,
            audience:jwtSettings.Audience,
            claims:claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryInMinutes),
            signingCredentials:creds
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
