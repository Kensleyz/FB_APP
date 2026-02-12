namespace PageBoostAI.Application.Common.Interfaces;

public record UnsplashPhoto(string Id, string Url, string SmallUrl, string AuthorName, string AuthorUrl);

public interface IUnsplashService
{
    Task<List<UnsplashPhoto>> SearchPhotosAsync(string query, int count = 10, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadPhotoAsync(string photoId, CancellationToken cancellationToken = default);
}
