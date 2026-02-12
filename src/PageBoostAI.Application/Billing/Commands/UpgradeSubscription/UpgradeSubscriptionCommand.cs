using MediatR;
using PageBoostAI.Application.Billing.DTOs;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Billing.Commands.UpgradeSubscription;

public record UpgradeSubscriptionCommand(string NewTier) : IRequest<Result<SubscriptionDto>>;
