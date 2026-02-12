using PageBoostAI.Domain.Enums;

namespace PageBoostAI.Application.Common.Interfaces;

public record PostVariation(string Content, List<string> Hashtags, string CallToAction);

public interface IAnthropicService
{
    Task<List<PostVariation>> GeneratePostsAsync(
        BusinessType businessType,
        ToneOption tone,
        PostType postType,
        string language,
        string businessName,
        string businessDescription,
        CancellationToken cancellationToken = default);
}
