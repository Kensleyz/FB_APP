using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Features.Billing.Commands;

public record UpgradeSubscriptionCommand(Guid UserId, string NewTier) : IRequest<Result<SubscriptionDto>>;

public class UpgradeSubscriptionCommandHandler : IRequestHandler<UpgradeSubscriptionCommand, Result<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;

    public UpgradeSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<SubscriptionDto>> Handle(UpgradeSubscriptionCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<SubscriptionTier>(request.NewTier, true, out var newTier))
            return Result<SubscriptionDto>.Failure("Invalid subscription tier.");

        var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken);
        if (subscription is null)
            return Result<SubscriptionDto>.Failure("No active subscription found.");

        var newAmount = GetTierPrice(newTier);
        var nextBilling = DateTime.UtcNow.AddMonths(1);

        subscription.Upgrade(newTier, newAmount, nextBilling);
        await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is not null)
        {
            user.UpdateSubscription(newTier, nextBilling);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        var dto = new SubscriptionDto(
            subscription.Id,
            subscription.Tier.ToString(),
            subscription.Amount.Amount,
            subscription.Currency,
            subscription.Status.ToString(),
            subscription.StartDate,
            subscription.NextBillingDate,
            subscription.CancelledAt);

        return Result<SubscriptionDto>.Success(dto);
    }

    private static Money GetTierPrice(SubscriptionTier tier) => tier switch
    {
        SubscriptionTier.Starter => Money.ZAR(99m),
        SubscriptionTier.Growth => Money.ZAR(249m),
        SubscriptionTier.Pro => Money.ZAR(499m),
        _ => Money.ZAR(0m)
    };
}
