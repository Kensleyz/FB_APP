using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;

    public ResetPasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(request.Token, cancellationToken);
        if (user is null)
            return Result<bool>.Failure("Invalid or expired reset token.");

        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.ResetPassword(newPasswordHash, request.Token);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return Result<bool>.Success(true);
    }
}
