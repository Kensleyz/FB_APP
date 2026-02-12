using FluentValidation;
using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Content.DTOs;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Content.Commands.GenerateImage;

public record GenerateImageCommand(
    string SearchQuery,
    string? OverlayText,
    bool OptimizeForFacebook = true) : IRequest<Result<GenerateImageResponseDto>>;

public class GenerateImageCommandValidator : AbstractValidator<GenerateImageCommand>
{
    public GenerateImageCommandValidator()
    {
        RuleFor(x => x.SearchQuery).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OverlayText).MaximumLength(100).When(x => x.OverlayText is not null);
    }
}

public class GenerateImageCommandHandler : IRequestHandler<GenerateImageCommand, Result<GenerateImageResponseDto>>
{
    private readonly IUnsplashService _unsplashService;
    private readonly IImageProcessingService _imageService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUsageMetricsRepository _usageRepository;
    private readonly IUserRepository _userRepository;

    public GenerateImageCommandHandler(
        IUnsplashService unsplashService,
        IImageProcessingService imageService,
        ICurrentUserService currentUser,
        IUsageMetricsRepository usageRepository,
        IUserRepository userRepository)
    {
        _unsplashService = unsplashService;
        _imageService = imageService;
        _currentUser = currentUser;
        _usageRepository = usageRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<GenerateImageResponseDto>> Handle(GenerateImageCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return Result<GenerateImageResponseDto>.Failure("Not authenticated.");

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (user is null)
            return Result<GenerateImageResponseDto>.Failure("User not found.");

        var metrics = await _usageRepository.GetOrCreateCurrentMonthAsync(user.Id, cancellationToken);
        var (_, _, maxImages) = Domain.Entities.UsageMetrics.GetLimits(user.SubscriptionTier);
        if (metrics.ImagesCreated >= maxImages)
            return Result<GenerateImageResponseDto>.Failure(
                $"Image generation limit reached for the {user.SubscriptionTier} plan.");

        var photos = await _unsplashService.SearchPhotosAsync(request.SearchQuery, 1, cancellationToken);
        if (photos.Count == 0)
            return Result<GenerateImageResponseDto>.Failure("No images found for the given query.");

        var photo = photos[0];
        var imageData = await _unsplashService.DownloadPhotoAsync(photo.Id, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.OverlayText))
            imageData = await _imageService.AddTextOverlayAsync(imageData, request.OverlayText, cancellationToken);

        if (request.OptimizeForFacebook)
            imageData = await _imageService.OptimizeForFacebookAsync(imageData, cancellationToken);

        metrics.IncrementImages();
        await _usageRepository.UpdateAsync(metrics, cancellationToken);

        var base64 = Convert.ToBase64String(imageData);
        return Result<GenerateImageResponseDto>.Success(
            new GenerateImageResponseDto(photo.Url, base64, photo.AuthorName, photo.AuthorUrl));
    }
}
