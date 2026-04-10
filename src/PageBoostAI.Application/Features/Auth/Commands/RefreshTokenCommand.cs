using System.Security.Claims;
using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponseDto>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtService.ValidateToken(request.RefreshToken);
        if (principal is null)
            return Result<AuthResponseDto>.Failure("Invalid or expired refresh token.");

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? principal.FindFirst("sub")?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Result<AuthResponseDto>.Failure("Invalid token.");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return Result<AuthResponseDto>.Failure("User not found.");

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email.Value, user.SubscriptionTier.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        var userDto = new UserProfileDto(
            user.Id, user.Email.Value, user.FirstName, user.LastName,
            user.PhoneNumber, user.SubscriptionTier.ToString(), user.IsEmailVerified,
            user.LastLoginAt, user.CreatedAt);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(accessToken, refreshToken, userDto));
    }
}
