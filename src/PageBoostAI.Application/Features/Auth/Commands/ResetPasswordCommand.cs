using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Auth.Commands;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<Result>;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public ResetPasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(request.Token, cancellationToken);
        if (user is null)
            return Result.Failure("Invalid or expired password reset token.");

        try
        {
            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.ResetPassword(newHash, request.Token);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _userRepository.UpdateAsync(user, cancellationToken);

        return Result.Success();
    }
}
