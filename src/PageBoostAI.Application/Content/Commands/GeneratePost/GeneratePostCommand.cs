using FluentValidation;
using MediatR;
using PageBoostAI.Application.Common.Interfaces;
using PageBoostAI.Application.Common.Models;
using PageBoostAI.Application.Content.DTOs;
using PageBoostAI.Domain.Enums;
using PageBoostAI.Domain.Exceptions;
using PageBoostAI.Domain.Interfaces;

namespace PageBoostAI.Application.Content.Commands.GeneratePost;

public record GeneratePostCommand(
    BusinessType BusinessType,
    ToneOption Tone,
    PostType PostType,
    string Language,
    string BusinessName,
    string BusinessDescription) : IRequest<Result<GeneratePostResponseDto>>;

public class GeneratePostCommandValidator : AbstractValidator<GeneratePostCommand>
{
    public GeneratePostCommandValidator()
    {
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BusinessDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Language).NotEmpty().MaximumLength(20);
    }
}

public class GeneratePostCommandHandler : IRequestHandler<GeneratePostCommand, Result<GeneratePostResponseDto>>
{
    private readonly IAnthropicService _anthropicService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUsageMetricsRepository _usageRepository;
    private readonly IUserRepository _userRepository;

    public GeneratePostCommandHandler(
        IAnthropicService anthropicService,
        ICurrentUserService currentUser,
        IUsageMetricsRepository usageRepository,
        IUserRepository userRepository)
    {
        _anthropicService = anthropicService;
        _currentUser = currentUser;
        _usageRepository = usageRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<GeneratePostResponseDto>> Handle(GeneratePostCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return Result<GeneratePostResponseDto>.Failure("Not authenticated.");

        var user = await _userRepository.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (user is null)
            return Result<GeneratePostResponseDto>.Failure("User not found.");

        var metrics = await _usageRepository.GetOrCreateCurrentMonthAsync(user.Id, cancellationToken);
        if (metrics.HasReachedLimit(user.SubscriptionTier))
            return Result<GeneratePostResponseDto>.Failure(
                $"Post generation limit reached for the {user.SubscriptionTier} plan.");

        var variations = await _anthropicService.GeneratePostsAsync(
            request.BusinessType, request.Tone, request.PostType,
            request.Language, request.BusinessName, request.BusinessDescription,
            cancellationToken);

        metrics.IncrementPosts();
        await _usageRepository.UpdateAsync(metrics, cancellationToken);

        var dtos = variations.Select(v => new PostVariationDto(v.Content, v.Hashtags, v.CallToAction)).ToList();
        return Result<GeneratePostResponseDto>.Success(new GeneratePostResponseDto(dtos));
    }
}
