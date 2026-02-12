using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Billing.Queries;

public record GetSubscriptionQuery(Guid UserId) : IRequest<Result<SubscriptionDto>>;

public class GetSubscriptionQueryHandler : IRequestHandler<GetSubscriptionQuery, Result<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetSubscriptionQueryHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Result<SubscriptionDto>> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken);
        if (subscription is null)
            return Result<SubscriptionDto>.Failure("No active subscription found.");

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
}
