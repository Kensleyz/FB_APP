using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.DTOs;

namespace PageBoostAI.Application.Features.Content.Commands;

public record GenerateImageCommand(
    Guid UserId,
    string Prompt,
    string Style
) : IRequest<Result<GeneratedImageDto>>;

public class GenerateImageCommandHandler : IRequestHandler<GenerateImageCommand, Result<GeneratedImageDto>>
{
    public async Task<Result<GeneratedImageDto>> Handle(GenerateImageCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement image generation logic
        await Task.CompletedTask;
        return Result<GeneratedImageDto>.Failure("Not implemented yet");
    }
}
