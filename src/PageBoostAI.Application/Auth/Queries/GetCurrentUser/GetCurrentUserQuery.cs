using MediatR;
using PageBoostAI.Application.Auth.DTOs;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<Result<UserDto>>;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserQueryHandler(IUserRepository userRepository, ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return Result<UserDto>.Failure("Not authenticated.");

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (user is null)
            return Result<UserDto>.Failure("User not found.");

        return Result<UserDto>.Success(new UserDto(
            user.Id, user.Email.Value, user.FirstName, user.LastName,
            user.PhoneNumber, user.SubscriptionTier, user.IsEmailVerified,
            user.LastLoginAt, user.CreatedAt));
    }
}
