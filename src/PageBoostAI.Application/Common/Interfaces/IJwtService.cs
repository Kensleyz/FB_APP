using System.Security.Claims;

namespace PageBoostAI.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email, string subscriptionTier);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}
