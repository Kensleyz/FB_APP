using MediatR;
using PageBoostAI.Application.Auth.DTOs;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Auth.Commands.RefreshToken;

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
        var userId = _jwtService.ValidateRefreshToken(request.RefreshToken);
        if (userId is null)
            return Result<AuthResponseDto>.Failure("Invalid or expired refresh token.");

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
            return Result<AuthResponseDto>.Failure("User not found.");

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var userDto = new UserDto(
            user.Id, user.Email.Value, user.FirstName, user.LastName,
            user.PhoneNumber, user.SubscriptionTier, user.IsEmailVerified,
            user.LastLoginAt, user.CreatedAt);

        return Result<AuthResponseDto>.Success(
            new AuthResponseDto(accessToken, refreshToken, userDto));
    }
}
