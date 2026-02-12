using MediatR;
using PageBoostAI.Application.Billing.DTOs;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Billing.Queries.GetUsage;

public record GetUsageQuery : IRequest<Result<UsageDto>>;
