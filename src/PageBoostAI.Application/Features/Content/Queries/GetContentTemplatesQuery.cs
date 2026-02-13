using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Content.Queries;

public record GetContentTemplatesQuery(Guid UserId) : IRequest<Result<List<ContentTemplateDto>>>;

public class GetContentTemplatesQueryHandler : IRequestHandler<GetContentTemplatesQuery, Result<List<ContentTemplateDto>>>
{
    public async Task<Result<List<ContentTemplateDto>>> Handle(GetContentTemplatesQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement get templates logic
        await Task.CompletedTask;
        return Result<List<ContentTemplateDto>>.Failure("Not implemented yet");
    }
}
