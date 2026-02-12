using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Facebook.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Facebook.Queries.GetPages;

public record GetPagesQuery : IRequest<Result<List<FacebookPageDto>>>;

public class GetPagesQueryHandler : IRequestHandler<GetPagesQuery, Result<List<FacebookPageDto>>>
{
    private readonly IFacebookPageRepository _pageRepository;
    private readonly ICurrentUserService _currentUser;

    public GetPagesQueryHandler(IFacebookPageRepository pageRepository, ICurrentUserService currentUser)
    {
        _pageRepository = pageRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<FacebookPageDto>>> Handle(GetPagesQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return Result<List<FacebookPageDto>>.Failure("Not authenticated.");

        var pages = await _pageRepository.GetByUserIdAsync(_currentUser.UserId.Value, cancellationToken);
        var dtos = pages.Where(p => p.IsActive).Select(p => new FacebookPageDto(
            p.Id, p.FacebookPageId, p.PageName, p.PageCategory,
            p.ProfilePictureUrl, p.FollowerCount, p.IsActive,
            p.ConnectedAt, p.LastSyncedAt)).ToList();

        return Result<List<FacebookPageDto>>.Success(dtos);
    }
}
