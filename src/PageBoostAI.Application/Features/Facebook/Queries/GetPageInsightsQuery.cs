using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Facebook.Queries;

public record GetPageInsightsQuery(Guid UserId, Guid PageId) : IRequest<Result<PageInsightsDto>>;

public class GetPageInsightsQueryHandler : IRequestHandler<GetPageInsightsQuery, Result<PageInsightsDto>>
{
    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IFacebookGraphService _facebookGraphService;
    private readonly IEncryptionService _encryptionService;

    public GetPageInsightsQueryHandler(
        IFacebookPageRepository facebookPageRepository,
        IFacebookGraphService facebookGraphService,
        IEncryptionService encryptionService)
    {
        _facebookPageRepository = facebookPageRepository;
        _facebookGraphService = facebookGraphService;
        _encryptionService = encryptionService;
    }

    public async Task<Result<PageInsightsDto>> Handle(GetPageInsightsQuery request, CancellationToken cancellationToken)
    {
        var page = await _facebookPageRepository.GetByIdAsync(request.PageId, cancellationToken);
        if (page is null || page.UserId != request.UserId)
            return Result<PageInsightsDto>.Failure("Page not found.");

        var accessToken = _encryptionService.Decrypt(page.PageAccessToken);
        var insights = await _facebookGraphService.GetInsightsAsync(accessToken, page.FacebookPageId, cancellationToken);

        var dto = new PageInsightsDto(
            page.Id,
            page.PageName,
            insights.Followers,
            insights.Followers, // TotalLikes ≈ followers for this API response
            page.FollowerCount, // PostsPublished tracked separately
            insights.Reach,
            (int)insights.EngagementRate);

        return Result<PageInsightsDto>.Success(dto);
    }
}
