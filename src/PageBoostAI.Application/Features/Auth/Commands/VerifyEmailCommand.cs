using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Auth.Commands;

public record VerifyEmailCommand(string Token) : IRequest<Result>;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public VerifyEmailCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailVerificationTokenAsync(request.Token, cancellationToken);
        if (user is null)
            return Result.Failure("Invalid or expired verification token.");

        if (user.IsEmailVerified)
            return Result.Success();

        user.VerifyEmail();
        await _userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }
}
