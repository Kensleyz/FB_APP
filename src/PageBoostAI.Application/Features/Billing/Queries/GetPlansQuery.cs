using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Billing.Queries;

public record GetPlansQuery : IRequest<Result<List<PlanDto>>>;

public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, Result<List<PlanDto>>>
{
    public Task<Result<List<PlanDto>>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = new List<PlanDto>
        {
            new("Free", "Free", 0m, "ZAR", 5, 1, 2, new List<string>
            {
                "5 AI-generated posts per month",
                "1 Facebook page",
                "2 AI images per month",
                "Basic scheduling"
            }),
            new("Starter", "Starter", 99m, "ZAR", 10, 1, 5, new List<string>
            {
                "10 AI-generated posts per month",
                "1 Facebook page",
                "5 AI images per month",
                "Advanced scheduling",
                "Basic analytics"
            }),
            new("Growth", "Growth", 249m, "ZAR", 30, 3, 15, new List<string>
            {
                "30 AI-generated posts per month",
                "3 Facebook pages",
                "15 AI images per month",
                "Advanced scheduling",
                "Full analytics",
                "Priority support"
            }),
            new("Pro", "Pro", 499m, "ZAR", 100, 10, 50, new List<string>
            {
                "100 AI-generated posts per month",
                "10 Facebook pages",
                "50 AI images per month",
                "Advanced scheduling",
                "Full analytics",
                "Priority support",
                "Custom branding"
            })
        };

        return Task.FromResult(Result<List<PlanDto>>.Success(plans));
    }
}
