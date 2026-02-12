using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public VerifyEmailCommandHandler(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<Result<bool>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailVerificationTokenAsync(request.Token, cancellationToken);
        if (user is null)
            return Result<bool>.Failure("Invalid verification token.");

        user.VerifyEmail();
        await _userRepository.UpdateAsync(user, cancellationToken);

        await _emailService.SendWelcomeEmailAsync(user.Email.Value, user.FirstName, cancellationToken);

        return Result<bool>.Success(true);
    }
}
