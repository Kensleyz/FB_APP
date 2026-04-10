using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Auth.Queries;

public record GetCurrentUserQuery(Guid UserId) : IRequest<Result<UserProfileDto>>;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserProfileDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<UserProfileDto>.Failure("User not found.");

        var dto = new UserProfileDto(
            user.Id,
            user.Email.Value,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.SubscriptionTier.ToString(),
            user.IsEmailVerified,
            user.LastLoginAt,
            user.CreatedAt);

        return Result<UserProfileDto>.Success(dto);
    }
}
