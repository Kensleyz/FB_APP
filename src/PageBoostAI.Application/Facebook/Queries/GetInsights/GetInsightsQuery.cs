using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Facebook.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Facebook.Queries.GetInsights;

public record GetInsightsQuery(Guid PageId) : IRequest<Result<PageInsightsDto>>;

public class GetInsightsQueryHandler : IRequestHandler<GetInsightsQuery, Result<PageInsightsDto>>
{
    private readonly IFacebookPageRepository _pageRepository;
    private readonly IFacebookGraphService _facebookService;
    private readonly IEncryptionService _encryptionService;
    private readonly ICurrentUserService _currentUser;

    public GetInsightsQueryHandler(
        IFacebookPageRepository pageRepository,
        IFacebookGraphService facebookService,
        IEncryptionService encryptionService,
        ICurrentUserService currentUser)
    {
        _pageRepository = pageRepository;
        _facebookService = facebookService;
        _encryptionService = encryptionService;
        _currentUser = currentUser;
    }

    public async Task<Result<PageInsightsDto>> Handle(GetInsightsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return Result<PageInsightsDto>.Failure("Not authenticated.");

        var page = await _pageRepository.GetByIdAsync(request.PageId, cancellationToken);
        if (page is null || page.UserId != _currentUser.UserId.Value)
            return Result<PageInsightsDto>.Failure("Page not found.");

        var decryptedToken = _encryptionService.Decrypt(page.PageAccessToken);
        var insights = await _facebookService.GetInsightsAsync(decryptedToken, page.FacebookPageId, cancellationToken);

        return Result<PageInsightsDto>.Success(
            new PageInsightsDto(insights.Followers, insights.EngagementRate, insights.Reach, insights.Impressions));
    }
}
