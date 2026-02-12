namespace PageBoostAI.Application.Common.Interfaces;

public interface IImageProcessingService
{
    Task<byte[]> AddTextOverlayAsync(byte[] imageData, string text, CancellationToken cancellationToken = default);
    Task<byte[]> AddLogoAsync(byte[] imageData, byte[] logoData, CancellationToken cancellationToken = default);
    Task<byte[]> OptimizeForFacebookAsync(byte[] imageData, CancellationToken cancellationToken = default);
}
