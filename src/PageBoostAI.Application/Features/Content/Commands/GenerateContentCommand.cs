using MediatR;
using PageBoostAI.Application.Common;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.DTOs;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Interfaces;

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
    private readonly IUserRepository _userRepository;
    private readonly IFacebookPageRepository _facebookPageRepository;
    private readonly IUsageMetricsRepository _usageMetricsRepository;
    private readonly IAIService _anthropicService;

    public GenerateContentCommandHandler(
        IUserRepository userRepository,
        IFacebookPageRepository facebookPageRepository,
        IUsageMetricsRepository usageMetricsRepository,
        IAIService anthropicService)
    {
        _userRepository = userRepository;
        _facebookPageRepository = facebookPageRepository;
        _usageMetricsRepository = usageMetricsRepository;
        _anthropicService = anthropicService;
    }

    public async Task<Result<GeneratedContentDto>> Handle(GenerateContentCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<GeneratedContentDto>.Failure("User not found.");

        var page = await _facebookPageRepository.GetByIdAsync(request.PageId, cancellationToken);
        if (page is null || page.UserId != request.UserId)
            return Result<GeneratedContentDto>.Failure("Page not found.");

        var metrics = await _usageMetricsRepository.GetOrCreateCurrentMonthAsync(request.UserId, cancellationToken);
        if (!user.CanPublishPost(metrics))
            return Result<GeneratedContentDto>.Failure("Monthly post generation limit reached. Please upgrade your plan.");

        if (!Enum.TryParse<BusinessType>(request.BusinessType, true, out var businessType))
            return Result<GeneratedContentDto>.Failure("Invalid business type.");

        if (!Enum.TryParse<ToneOption>(request.Tone, true, out var tone))
            return Result<GeneratedContentDto>.Failure("Invalid tone option.");

        if (!Enum.TryParse<PostType>(request.PostType, true, out var postType))
            return Result<GeneratedContentDto>.Failure("Invalid post type.");

        var variations = await _anthropicService.GeneratePostsAsync(
            businessType,
            tone,
            postType,
            language: "English",
            businessName: page.PageName,
            businessDescription: request.AdditionalContext ?? string.Empty,
            cancellationToken);

        if (variations.Count == 0)
            return Result<GeneratedContentDto>.Failure("Failed to generate content. Please try again.");

        var best = variations[0];

        metrics.IncrementPosts();
        await _usageMetricsRepository.UpdateAsync(metrics, cancellationToken);

        return Result<GeneratedContentDto>.Success(
            new GeneratedContentDto(best.Content, null, best.Hashtags));
    }
}
