using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Facebook.Queries;

public record GetUserPagesQuery(Guid UserId) : IRequest<Result<List<FacebookPageDto>>>;

public class GetUserPagesQueryHandler : IRequestHandler<GetUserPagesQuery, Result<List<FacebookPageDto>>>
{
    private readonly IFacebookPageRepository _facebookPageRepository;

    public GetUserPagesQueryHandler(IFacebookPageRepository facebookPageRepository)
    {
        _facebookPageRepository = facebookPageRepository;
    }

    public async Task<Result<List<FacebookPageDto>>> Handle(GetUserPagesQuery request, CancellationToken cancellationToken)
    {
        var pages = await _facebookPageRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var dtos = pages
            .Where(p => p.IsActive)
            .Select(p => new FacebookPageDto(
                p.Id,
                p.FacebookPageId,
                p.PageName,
                p.PageCategory,
                p.ProfilePictureUrl,
                p.FollowerCount,
                p.IsActive,
                p.ConnectedAt,
                p.LastSyncedAt))
            .ToList();

        return Result<List<FacebookPageDto>>.Success(dtos);
    }
}
