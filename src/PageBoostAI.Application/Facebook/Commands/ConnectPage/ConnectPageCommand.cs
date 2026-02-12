using FluentValidation;
using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Facebook.DTOs;
using PageBoostAI.Domain.Entities;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Facebook.Commands.ConnectPage;

public record ConnectPageCommand(string AuthCode) : IRequest<Result<List<FacebookPageDto>>>;

public class ConnectPageCommandValidator : AbstractValidator<ConnectPageCommand>
{
    public ConnectPageCommandValidator()
    {
        RuleFor(x => x.AuthCode).NotEmpty().WithMessage("Authorization code is required.");
    }
}

public class ConnectPageCommandHandler : IRequestHandler<ConnectPageCommand, Result<List<FacebookPageDto>>>
{
    private readonly IFacebookGraphService _facebookService;
    private readonly IFacebookPageRepository _pageRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserRepository _userRepository;

    public ConnectPageCommandHandler(
        IFacebookGraphService facebookService,
        IFacebookPageRepository pageRepository,
        IEncryptionService encryptionService,
        ICurrentUserService currentUser,
        IUserRepository userRepository)
    {
        _facebookService = facebookService;
        _pageRepository = pageRepository;
        _encryptionService = encryptionService;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    public async Task<Result<List<FacebookPageDto>>> Handle(ConnectPageCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return Result<List<FacebookPageDto>>.Failure("Not authenticated.");

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (user is null)
            return Result<List<FacebookPageDto>>.Failure("User not found.");

        var (maxPosts, maxPages, maxImages) = UsageMetrics.GetLimits(user.SubscriptionTier);
        var currentPageCount = await _pageRepository.CountByUserIdAsync(user.Id, cancellationToken);
        if (currentPageCount >= maxPages)
            return Result<List<FacebookPageDto>>.Failure(
                $"Page limit reached for the {user.SubscriptionTier} plan. Max: {maxPages}.");

        var shortToken = await _facebookService.ExchangeCodeForTokenAsync(request.AuthCode, cancellationToken);
        var (longToken, expiresAt) = await _facebookService.GetLongLivedTokenAsync(shortToken, cancellationToken);
        var pages = await _facebookService.GetPagesAsync(longToken, cancellationToken);

        var result = new List<FacebookPageDto>();
        foreach (var pageInfo in pages)
        {
            var existing = await _pageRepository.GetByFacebookPageIdAsync(pageInfo.PageId, cancellationToken);
            if (existing is not null) continue;

            var encryptedToken = _encryptionService.Encrypt(pageInfo.AccessToken);
            var page = new FacebookPage(
                user.Id, pageInfo.PageId, pageInfo.PageName,
                encryptedToken, pageInfo.Category, pageInfo.ProfilePictureUrl);

            await _pageRepository.AddAsync(page, cancellationToken);
            result.Add(new FacebookPageDto(
                page.Id, page.FacebookPageId, page.PageName, page.PageCategory,
                page.ProfilePictureUrl, page.FollowerCount, page.IsActive,
                page.ConnectedAt, page.LastSyncedAt));
        }

        return Result<List<FacebookPageDto>>.Success(result);
    }
}
