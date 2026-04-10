using PageBoostAI.Application.Common.Interfaces;

namespace PageBoostAI.Infrastructure.ExternalServices;

public class ImageProcessingService : IImageProcessingService
{
    public Task<byte[]> AddTextOverlayAsync(byte[] imageData, string text, CancellationToken cancellationToken = default)
        => Task.FromResult(imageData);

    public Task<byte[]> AddLogoAsync(byte[] imageData, byte[] logoData, CancellationToken cancellationToken = default)
        => Task.FromResult(imageData);

    public Task<byte[]> OptimizeForFacebookAsync(byte[] imageData, CancellationToken cancellationToken = default)
        => Task.FromResult(imageData);
}
