using Backend.Domain.Enums;

namespace Backend.Application.Interfaces;

public record UploadInfo(string ObjectKey, string Url);

public interface IImageStorageService
{
    string GenerateObjectKey(StorageDestination dest, Guid id, string originalName);
    string GenerateUserBookObjectKey(Guid userId, Guid userBookId, string originalName);

    Task<string> GenerateUploadUrlAsync(string objectKey, string? bucketName = null);

    Task<string> GenerateSignedDownloadUrlAsync(string objectKey, TimeSpan expiration, string? bucketName = null);

    string GetPublicUrl(string objectKey);
    string GetThumbnailUrl(string objectKey);

    Task<bool> ExistsAsync(
        string objectKey,
        CancellationToken cancellationToken = default,
        string? bucketName = null);
    
    Task DeleteAsync(string objectKey, CancellationToken ct = default, string? bucketName = null);
}
