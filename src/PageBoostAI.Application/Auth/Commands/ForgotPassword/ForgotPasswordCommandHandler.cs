using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        // Always return success to prevent email enumeration
        if (user is null)
            return Result<bool>.Success(true);

        user.SetPasswordResetToken();
        await _userRepository.UpdateAsync(user, cancellationToken);

        await _emailService.SendPasswordResetEmailAsync(
            email.Value, user.PasswordResetToken!, cancellationToken);

        return Result<bool>.Success(true);
    }
}
