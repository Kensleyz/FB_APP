using MediatR;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Billing.Commands.Subscribe;

public record SubscribeCommand(string Tier, string? ReturnUrl, string? CancelUrl) : IRequest<Result<string>>;
