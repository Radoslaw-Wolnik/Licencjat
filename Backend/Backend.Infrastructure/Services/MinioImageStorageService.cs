using Minio;
using Minio.Exceptions;
using Backend.Application.Interfaces;
using Minio.DataModel.Args;
using Microsoft.Extensions.Options;
using Backend.Infrastructure.Configuration;
using Backend.Domain.Enums;
using Minio.DataModel;
using System.Reactive.Linq;

namespace Backend.Infrastructure.Services;

public class MinioImageStorageService : IImageStorageService
{
    private readonly IMinioClient _client;
    private readonly MinioSettings _settings;

    public MinioImageStorageService(IMinioClient client, IOptions<MinioSettings> options)
    {
        _client   = client;
        _settings = options.Value;
    }

    public string GenerateObjectKey(StorageDestination dest, Guid id, string originalName)
    {
        if (dest == StorageDestination.UserBooks) throw new Exception("Used wrong function! Use GenerateUserBookObjectKey ");

        var ext = Path.GetExtension(originalName);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";

        var fileName = $"{Guid.NewGuid()}{ext}";
        return $"{dest.ToPath()}/{id}/{fileName}";
    }

    public string GenerateUserBookObjectKey(Guid userId, Guid userBookId, string originalName)
    {
        var ext = Path.GetExtension(originalName);
            if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";
        
        var fileName = $"{Guid.NewGuid()}{ext}";
        return $"{StorageDestination.UserBooks.ToPath()}/{userId}/{userBookId}/{fileName}";
    }
    
    public async Task<string> GenerateUploadUrlAsync(string objectKey, string? bucketName = null)
    {
        bucketName ??= _settings.BucketName;

        var presignedUrl = await _client.PresignedPutObjectAsync(
            new PresignedPutObjectArgs()
                .WithBucket(bucketName) // or here bucketName ??_settings.BucketName;
                .WithObject(objectKey)
                .WithExpiry(_settings.ExpiryMinutes * 60));
        
        return presignedUrl;
    }
    
    public async Task<string> GenerateSignedDownloadUrlAsync(string objectKey, TimeSpan expiration, string? bucketName = null)
    {
        var presignedUrl = await _client.PresignedGetObjectAsync(
            new PresignedGetObjectArgs()
                .WithBucket(bucketName??_settings.BucketName)
                .WithObject(objectKey)
                .WithExpiry((int)expiration.TotalSeconds));
        return presignedUrl;
    }

    public string GetPublicUrl(string objectKey)
        => $"{_settings.PublicBaseUrl.TrimEnd('/')}/{objectKey}";
    
    public string GetThumbnailUrl(string objectKey)
    {
        var thumbKey = Path.ChangeExtension(objectKey, null) + "-thumb.jpg";
        return $"{_settings.PublicBaseUrl.TrimEnd('/')}/{thumbKey}";
    }


    public async Task<bool> ExistsAsync(
        string objectKey,
        CancellationToken cancellationToken = default,
        string? bucketName = null)
    {
        bucketName ??= _settings.BucketName;

        try
        {
            // Use ListObjects to verify existence
            var listArgs = new ListObjectsArgs()
                .WithBucket(bucketName)
                .WithPrefix(objectKey)
                .WithRecursive(false);

            var found = false;
            var observable = _client.ListObjectsAsync(listArgs, cancellationToken);
            
            await observable.ForEachAsync(item => 
            {
                if (item.Key == objectKey) found = true;
            }, cancellationToken);

            return found;
        }
        catch (MinioException ex)
        {
            throw new Exception(
                $"Could not check existence of '{objectKey}': {ex.Message}", ex);
        }
    }
    
    public async Task DeleteAsync(string objectKey, CancellationToken ct = default, string? bucket = null)
    {
        bucket ??= _settings.BucketName;
        // delete the main object
        await _client.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectKey),
            ct);

        // also attempt to delete thumbnail if exists
        var thumbKey = Path.ChangeExtension(objectKey, null) + "-thumb.jpg";
        try
        {
            await _client.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(thumbKey),
                ct);
        }
        catch (ObjectNotFoundException)
        {
            // thumbnail wasn’t there; ignore
        }
    }

}