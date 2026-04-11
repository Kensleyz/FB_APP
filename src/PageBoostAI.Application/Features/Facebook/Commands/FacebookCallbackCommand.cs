using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Facebook.Commands;

public record FacebookCallbackCommand(Guid UserId, string Code, string? State) : IRequest<Result<List<FacebookPageDto>>>;

public class FacebookCallbackCommandHandler : IRequestHandler<FacebookCallbackCommand, Result<List<FacebookPageDto>>>
{
    private readonly IFacebookGraphService _facebookGraphService;
    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IEncryptionService _encryptionService;

    public FacebookCallbackCommandHandler(
        IFacebookGraphService facebookGraphService,
        IFacebookPageRepository facebookPageRepository,
        IEncryptionService encryptionService)
    {
        _facebookGraphService = facebookGraphService;
        _facebookPageRepository = facebookPageRepository;
        _encryptionService = encryptionService;
    }

    public async Task<Result<List<FacebookPageDto>>> Handle(FacebookCallbackCommand request, CancellationToken cancellationToken)
    {
        // Exchange auth code for short-lived token, then upgrade to long-lived
        string shortLivedToken;
        try
        {
            shortLivedToken = await _facebookGraphService.ExchangeCodeForTokenAsync(request.Code, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<List<FacebookPageDto>>.Failure($"Failed to exchange auth code: {ex.Message}");
        }

        var (longLivedToken, expiresAt) = await _facebookGraphService.GetLongLivedTokenAsync(shortLivedToken, cancellationToken);

        // Get the list of pages the user manages
        var pageInfos = await _facebookGraphService.GetPagesAsync(longLivedToken, cancellationToken);
        if (pageInfos.Count == 0)
            return Result<List<FacebookPageDto>>.Failure("No Facebook pages found. Make sure you manage at least one Facebook Page.");

        var resultDtos = new List<FacebookPageDto>();

        foreach (var pageInfo in pageInfos)
        {
            // Upsert: update existing page or create new one
            var existing = await _facebookPageRepository.GetByFacebookPageIdAsync(pageInfo.PageId, cancellationToken);
            if (existing is not null)
            {
                var encryptedToken = _encryptionService.Encrypt(pageInfo.AccessToken);
                existing.RefreshToken(encryptedToken, expiresAt);
                await _facebookPageRepository.UpdateAsync(existing, cancellationToken);

                resultDtos.Add(new FacebookPageDto(existing.Id, existing.FacebookPageId, existing.PageName,
                    existing.PageCategory, existing.ProfilePictureUrl, existing.FollowerCount, existing.IsActive, existing.ConnectedAt, existing.LastSyncedAt));
            }
            else
            {
                var encryptedToken = _encryptionService.Encrypt(pageInfo.AccessToken);
                var page = new FacebookPage(
                    request.UserId,
                    pageInfo.PageId,
                    pageInfo.PageName,
                    encryptedToken,
                    pageInfo.Category,
                    pageInfo.ProfilePictureUrl);

                await _facebookPageRepository.AddAsync(page, cancellationToken);

                resultDtos.Add(new FacebookPageDto(page.Id, page.FacebookPageId, page.PageName,
                    page.PageCategory, page.ProfilePictureUrl, page.FollowerCount, page.IsActive, page.ConnectedAt, page.LastSyncedAt));
            }
        }

        return Result<List<FacebookPageDto>>.Success(resultDtos);
    }
}
