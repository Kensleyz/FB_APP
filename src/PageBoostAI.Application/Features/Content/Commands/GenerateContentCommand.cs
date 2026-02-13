using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Content.Commands;

public record GenerateContentCommand(
    Guid UserId,
    Guid PageId,
    string BusinessType,
    string Tone,
    string PostType,
    string? AdditionalContext
) : IRequest<Result<GeneratedContentDto>>;

public class GenerateContentCommandHandler : IRequestHandler<GenerateContentCommand, Result<GeneratedContentDto>>
{
    public async Task<Result<GeneratedContentDto>> Handle(GenerateContentCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement content generation logic
        await Task.CompletedTask;
        return Result<GeneratedContentDto>.Failure("Not implemented yet");
    }
}
