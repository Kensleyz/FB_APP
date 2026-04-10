using MediatR;
using PageBoostAI.Application.Auth.DTOs;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IJwtService jwtService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            return Result<AuthResponseDto>.Failure("An account with this email already exists.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(email, passwordHash, request.FirstName, request.LastName, request.PhoneNumber);

        await _userRepository.AddAsync(user, cancellationToken);

        await _emailService.SendVerificationEmailAsync(
            email.Value, user.EmailVerificationToken!, cancellationToken);

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email.Value, user.SubscriptionTier.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        var userDto = new UserDto(
            user.Id, user.Email.Value, user.FirstName, user.LastName,
            user.PhoneNumber, user.SubscriptionTier, user.IsEmailVerified,
            user.LastLoginAt, user.CreatedAt);

        return Result<AuthResponseDto>.Success(
            new AuthResponseDto(accessToken, refreshToken, userDto));
    }
}
