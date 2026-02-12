using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Facebook.Commands.DisconnectPage;

public record DisconnectPageCommand(Guid PageId) : IRequest<Result<bool>>;

public class DisconnectPageCommandHandler : IRequestHandler<DisconnectPageCommand, Result<bool>>
{
    private readonly IFacebookPageRepository _pageRepository;
    private readonly ICurrentUserService _currentUser;

    public DisconnectPageCommandHandler(IFacebookPageRepository pageRepository, ICurrentUserService currentUser)
    {
        _pageRepository = pageRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<bool>> Handle(DisconnectPageCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return Result<bool>.Failure("Not authenticated.");

        var page = await _pageRepository.GetByIdAsync(request.PageId, cancellationToken);
        if (page is null || page.UserId != _currentUser.UserId.Value)
            return Result<bool>.Failure("Page not found.");

        page.Deactivate();
        await _pageRepository.UpdateAsync(page, cancellationToken);
        return Result<bool>.Success(true);
    }
}
