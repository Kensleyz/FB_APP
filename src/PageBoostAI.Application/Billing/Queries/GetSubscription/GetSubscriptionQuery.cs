using MediatR;
using PageBoostAI.Application.Billing.DTOs;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Billing.Queries.GetSubscription;

public record GetSubscriptionQuery : IRequest<Result<SubscriptionDto>>;
