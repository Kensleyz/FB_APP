using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Facebook.Commands;

public record DisconnectPageCommand(Guid UserId, Guid PageId) : IRequest<Result>;

public class DisconnectPageCommandHandler : IRequestHandler<DisconnectPageCommand, Result>
{
    private readonly IFacebookPageRepository _facebookPageRepository;

    public DisconnectPageCommandHandler(IFacebookPageRepository facebookPageRepository)
    {
        _facebookPageRepository = facebookPageRepository;
    }

    public async Task<Result> Handle(DisconnectPageCommand request, CancellationToken cancellationToken)
    {
        var page = await _facebookPageRepository.GetByIdAsync(request.PageId, cancellationToken);
        if (page is null || page.UserId != request.UserId)
            return Result.Failure("Page not found.");

        page.Deactivate();
        await _facebookPageRepository.UpdateAsync(page, cancellationToken);

        return Result.Success();
    }
}
