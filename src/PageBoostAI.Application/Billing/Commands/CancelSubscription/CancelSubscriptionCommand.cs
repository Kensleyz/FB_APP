using MediatR;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Billing.Commands.CancelSubscription;

public record CancelSubscriptionCommand : IRequest<Result<bool>>;
