using PageBoostAI.Domain.Entities;

namespace PageBoostAI.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid? ValidateRefreshToken(string refreshToken);
}
