using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Features.Content.Commands;

public record GenerateImageCommand(
    Guid UserId,
    string Prompt,
    string Style
) : IRequest<Result<GeneratedImageDto>>;

public class GenerateImageCommandHandler : IRequestHandler<GenerateImageCommand, Result<GeneratedImageDto>>
{
    private readonly IUsageMetricsRepository _usageMetricsRepository;
    private readonly IUnsplashService _unsplashService;
    private readonly IUserRepository _userRepository;

    public GenerateImageCommandHandler(
        IUserRepository userRepository,
        IUsageMetricsRepository usageMetricsRepository,
        IUnsplashService unsplashService)
    {
        _userRepository = userRepository;
        _usageMetricsRepository = usageMetricsRepository;
        _unsplashService = unsplashService;
    }

    public async Task<Result<GeneratedImageDto>> Handle(GenerateImageCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<GeneratedImageDto>.Failure("User not found.");

        var metrics = await _usageMetricsRepository.GetOrCreateCurrentMonthAsync(request.UserId, cancellationToken);

        var query = string.IsNullOrWhiteSpace(request.Style)
            ? request.Prompt
            : $"{request.Prompt} {request.Style}";

        var photos = await _unsplashService.SearchPhotosAsync(query, count: 5, cancellationToken);
        if (photos.Count == 0)
            return Result<GeneratedImageDto>.Failure("No images found for the given prompt. Try a different description.");

        var photo = photos[0];

        metrics.IncrementImages();
        await _usageMetricsRepository.UpdateAsync(metrics, cancellationToken);

        return Result<GeneratedImageDto>.Success(new GeneratedImageDto(photo.Url, request.Prompt));
    }
}
