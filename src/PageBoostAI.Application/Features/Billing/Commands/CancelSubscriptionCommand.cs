using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Billing.Commands;

public record CancelSubscriptionCommand(Guid UserId) : IRequest<Result>;

public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, Result>
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;

    public CancelSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken);
        if (subscription is null)
            return Result.Failure("No active subscription found.");

        subscription.Cancel();
        await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is not null)
        {
            user.UpdateSubscription(SubscriptionTier.Free);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        return Result.Success();
    }
}
