using MediatR;
using PageBoostAI.Application.Billing.DTOs;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Billing.Commands.DowngradeSubscription;

public record DowngradeSubscriptionCommand(string NewTier) : IRequest<Result<SubscriptionDto>>;
