using MediatR;
using PageBoostAI.Application.Billing.DTOs;
using PageBoostAI.Application.Common.Models;

namespace PageBoostAI.Application.Billing.Queries.GetPlans;

public record GetPlansQuery : IRequest<Result<List<PlanDto>>>;
