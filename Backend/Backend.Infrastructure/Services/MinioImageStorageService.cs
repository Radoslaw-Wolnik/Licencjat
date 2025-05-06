using Minio;
using Minio.Exceptions;
using Microsoft.Extensions.Configuration;
using Backend.Application.Interfaces;
using Minio.DataModel.Args;

namespace Backend.Infrastructure.Services;

public class MinioImageStorageService : IImageStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    
    public MinioImageStorageService(IConfiguration configuration)
    {
        var endpoint = configuration["Minio:Endpoint"];
        var accessKey = configuration["Minio:AccessKey"];
        var secretKey = configuration["Minio:SecretKey"];
        _bucketName = configuration["Minio:BucketName"] ?? "app-images";

        _minioClient = new MinioClient()
                         .WithEndpoint(endpoint)
                         .WithCredentials(accessKey, secretKey)
                         .Build(); // Build() returns IMinioClient
    }
    
    private async Task CreateBucketIfNotExistsAsync(string bucketName)
    {
        try
        {
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }
        }
        catch (MinioException ex)
        {
            throw new Exception("Error creating or checking bucket", ex);
        }
    }
    
    public async Task<string> GenerateProfilePictureUploadUrlAsync(Guid userId, string fileName, TimeSpan expiration)
    {
        var objectKey = $"users/{userId}/profile/{fileName}";
        var presignedUrl = await _minioClient.PresignedPutObjectAsync(
            new PresignedPutObjectArgs() // Correct namespace
                .WithBucket(_bucketName)
                .WithObject(objectKey)
                .WithExpiry((int)expiration.TotalSeconds));
        return presignedUrl;
    }
    
    public async Task<string> GenerateUserBookCoverUploadUrlAsync(Guid userId, string fileName, TimeSpan expiration)
    {
        var objectKey = $"users/{userId}/bookcovers/{fileName}";
        var presignedUrl = await _minioClient.PresignedPutObjectAsync(
            new PresignedPutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectKey)
                .WithExpiry((int)expiration.TotalSeconds));
        return presignedUrl;
    }
    
    public async Task<string> GenerateDownloadUrlAsync(string objectKey, TimeSpan expiration)
    {
        var presignedUrl = await _minioClient.PresignedGetObjectAsync(
            new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectKey)
                .WithExpiry((int)expiration.TotalSeconds));
        return presignedUrl;
    }
}

/*
user bucket
users/{userId}/profile/
users/{userId}/bookcovers/
admin bucket
books/{bookId}/covers/
*/