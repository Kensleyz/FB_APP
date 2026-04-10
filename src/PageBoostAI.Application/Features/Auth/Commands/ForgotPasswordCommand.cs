using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        // Always return success to avoid email enumeration
        if (user is null)
            return Result.Success();

        user.SetPasswordResetToken();
        await _userRepository.UpdateAsync(user, cancellationToken);

        await _emailService.SendPasswordResetEmailAsync(user.Email.Value, user.PasswordResetToken!, cancellationToken);

        return Result.Success();
    }
}
