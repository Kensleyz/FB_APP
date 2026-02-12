namespace PageBoostAI.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string verificationToken, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken = default);
    Task SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default);
}
