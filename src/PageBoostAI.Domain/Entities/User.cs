using PageBoostAI.Domain.Common;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Events;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Domain.Entities;

public sealed class User : BaseEntity
{
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public SubscriptionTier SubscriptionTier { get; private set; } = SubscriptionTier.Free;
    public DateTime? SubscriptionExpiresAt { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetExpiry { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private User() { } // EF Core

    public User(Email email, string passwordHash, string firstName, string lastName, string? phoneNumber = null)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        EmailVerificationToken = Guid.NewGuid().ToString("N");

        AddDomainEvent(new UserRegisteredEvent(Id, email.Value));
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        EmailVerificationToken = null;
        SetUpdated();
    }

    public void SetPasswordResetToken()
    {
        PasswordResetToken = Guid.NewGuid().ToString("N");
        PasswordResetExpiry = DateTime.UtcNow.AddHours(24);
        SetUpdated();
    }

    public void ResetPassword(string newPasswordHash, string token)
    {
        if (PasswordResetToken != token)
            throw new DomainException("Invalid password reset token.");
        if (PasswordResetExpiry < DateTime.UtcNow)
            throw new DomainException("Password reset token has expired.");

        PasswordHash = newPasswordHash;
        PasswordResetToken = null;
        PasswordResetExpiry = null;
        SetUpdated();
    }

    public void UpdateSubscription(SubscriptionTier newTier, DateTime? expiresAt = null)
    {
        var oldTier = SubscriptionTier;
        SubscriptionTier = newTier;
        SubscriptionExpiresAt = expiresAt;
        SetUpdated();

        AddDomainEvent(new SubscriptionChangedEvent(Id, oldTier, newTier));
    }

    public bool CanPublishPost(UsageMetrics? metrics)
    {
        if (metrics is null) return true;
        return !metrics.HasReachedLimit(SubscriptionTier);
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        SetUpdated();
    }
}
