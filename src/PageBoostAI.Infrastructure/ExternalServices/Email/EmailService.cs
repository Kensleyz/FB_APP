using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PageBoostAI.Application.Common.Interfaces;

namespace PageBoostAI.Infrastructure.ExternalServices.Email;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _frontendUrl;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _logger = logger;
        _frontendUrl = (configuration["FRONTEND_URL"] ?? string.Empty).TrimEnd('/');

        var smtp = configuration.GetSection("SmtpSettings");
        _fromEmail = configuration["SMTP_FROM_EMAIL"] ?? smtp["FromEmail"] ?? "noreply@pageboost.ai";
        _fromName = configuration["SMTP_FROM_NAME"] ?? smtp["FromName"] ?? "PageBoost AI";

        var host = configuration["SMTP_HOST"] ?? smtp["Host"] ?? "localhost";
        var port = int.Parse(configuration["SMTP_PORT"] ?? smtp["Port"] ?? "587");
        var username = configuration["SMTP_USERNAME"] ?? smtp["Username"];
        var password = configuration["SMTP_PASSWORD"] ?? smtp["Password"];

        _smtpClient = new SmtpClient(host, port)
        {
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)
            && password != "CHANGE-ME")
        {
            _smtpClient.Credentials = new NetworkCredential(username, password);
        }
    }

    public async Task SendVerificationEmailAsync(string email, string verificationToken, CancellationToken cancellationToken = default)
    {
        var verifyUrl = $"{_frontendUrl}/verify-email?token={verificationToken}";
        var subject = "Verify your PageBoost AI email";
        var body = $"""
            <h2>Welcome to PageBoost AI!</h2>
            <p>Please verify your email address by clicking the link below:</p>
            <p><a href="{verifyUrl}">Verify Email</a></p>
            <p>If you didn't create an account, you can safely ignore this email.</p>
            <p>Regards,<br/>The PageBoost AI Team</p>
            """;

        await SendAsync(email, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken = default)
    {
        var resetUrl = $"{_frontendUrl}/reset-password?token={resetToken}";
        var subject = "Reset your PageBoost AI password";
        var body = $"""
            <h2>Password Reset Request</h2>
            <p>Click the link below to reset your password:</p>
            <p><a href="{resetUrl}">Reset Password</a></p>
            <p>This link will expire in 24 hours.</p>
            <p>If you didn't request this, you can safely ignore this email.</p>
            <p>Regards,<br/>The PageBoost AI Team</p>
            """;

        await SendAsync(email, subject, body, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default)
    {
        var dashboardUrl = $"{_frontendUrl}/dashboard";
        var subject = "Welcome to PageBoost AI!";
        var body = $"""
            <h2>Welcome, {firstName}!</h2>
            <p>Thank you for joining PageBoost AI. We're excited to help you grow your Facebook presence.</p>
            <p>Here's what you can do:</p>
            <ul>
                <li>Connect your Facebook pages</li>
                <li>Generate AI-powered posts tailored for South African audiences</li>
                <li>Schedule and automate your content</li>
                <li>Track performance with analytics</li>
            </ul>
            <p>Get started now at <a href="{dashboardUrl}">your dashboard</a>.</p>
            <p>Regards,<br/>The PageBoost AI Team</p>
            """;

        await SendAsync(email, subject, body, cancellationToken);
    }

    private async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(to));

            await _smtpClient.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email sent to {Email}, subject: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}, subject: {Subject}", to, subject);
        }
    }
}
