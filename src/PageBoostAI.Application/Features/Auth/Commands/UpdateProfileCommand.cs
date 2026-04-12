using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Auth.Commands;

public record UpdateProfileCommand(Guid UserId, string FirstName, string LastName, string? PhoneNumber)
    : IRequest<Result<UserProfileDto>>;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;

    public UpdateProfileCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<UserProfileDto>.Failure("User not found.");

        user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber);
        await _userRepository.UpdateAsync(user, cancellationToken);

        var dto = new UserProfileDto(
            user.Id, user.Email.Value, user.FirstName, user.LastName,
            user.PhoneNumber, user.SubscriptionTier.ToString(), user.IsEmailVerified,
            user.LastLoginAt, user.CreatedAt);

        return Result<UserProfileDto>.Success(dto);
    }
}
