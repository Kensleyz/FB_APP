using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;
using PageBoostAI.Domain.ValueObjects;

namespace PageBoostAI.Application.Features.Billing.Commands;

public record SubscribeCommand(Guid UserId, string Tier, string? ReturnUrl, string? CancelUrl) : IRequest<Result<string>>;

public class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscribeCommandHandler(
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Result<string>> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<string>.Failure("User not found.");

        if (!Enum.TryParse<SubscriptionTier>(request.Tier, true, out var tier))
            return Result<string>.Failure("Invalid subscription tier.");

        if (tier == SubscriptionTier.Free)
            return Result<string>.Failure("Cannot subscribe to the Free tier.");

        var existing = await _subscriptionRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken);
        if (existing is not null)
            return Result<string>.Failure("User already has an active subscription. Cancel or upgrade instead.");

        var amount = GetTierPrice(tier);
        var nextBilling = DateTime.UtcNow.AddMonths(1);

        var subscription = new Subscription(
            request.UserId,
            tier,
            amount,
            payFastSubscriptionToken: null,
            nextBillingDate: nextBilling);

        await _subscriptionRepository.AddAsync(subscription, cancellationToken);

        user.UpdateSubscription(tier, nextBilling);
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Return subscription ID; payment integration handled via PayFast webhook flow
        return Result<string>.Success(subscription.Id.ToString());
    }

    private static Money GetTierPrice(SubscriptionTier tier) => tier switch
    {
        SubscriptionTier.Starter => Money.ZAR(99m),
        SubscriptionTier.Growth => Money.ZAR(249m),
        SubscriptionTier.Pro => Money.ZAR(499m),
        _ => Money.ZAR(0m)
    };
}
