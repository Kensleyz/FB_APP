using MediatR;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Billing.Queries;

public record GetUsageQuery(Guid UserId) : IRequest<Result<UsageDto>>;

public class GetUsageQueryHandler : IRequestHandler<GetUsageQuery, Result<UsageDto>>
{
    private readonly IUsageMetricsRepository _usageMetricsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFacebookPageRepository _facebookPageRepository;

    public GetUsageQueryHandler(
        IUsageMetricsRepository usageMetricsRepository,
        IUserRepository userRepository,
        IFacebookPageRepository facebookPageRepository)
    {
        _usageMetricsRepository = usageMetricsRepository;
        _userRepository = userRepository;
        _facebookPageRepository = facebookPageRepository;
    }

    public async Task<Result<UsageDto>> Handle(GetUsageQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<UsageDto>.Failure("User not found.");

        var currentPeriod = DateTime.UtcNow.ToString("yyyy-MM");
        var metrics = await _usageMetricsRepository.GetByUserIdAndPeriodAsync(request.UserId, currentPeriod, cancellationToken);

        var (maxPosts, maxPages, maxImages) = UsageMetrics.GetLimits(user.SubscriptionTier);
        var pagesConnected = await _facebookPageRepository.CountByUserIdAsync(request.UserId, cancellationToken);

        var dto = new UsageDto(
            currentPeriod,
            metrics?.PostsGenerated ?? 0,
            maxPosts,
            metrics?.ImagesCreated ?? 0,
            maxImages,
            metrics?.PostsPublished ?? 0,
            pagesConnected,
            maxPages);

        return Result<UsageDto>.Success(dto);
    }
}
