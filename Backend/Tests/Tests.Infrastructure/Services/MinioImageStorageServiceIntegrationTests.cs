using System.Net;
using Backend.Infrastructure.Configuration;
using Backend.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;


namespace Tests.Infrastructure.Services;

[Collection("Sequential")]
public class MinioImageStorageServiceIntegrationTests : TestContainersBase, IAsyncLifetime
{
    private MinioImageStorageService _service = null!;
    private string _bucketName = "test-bucket";



    protected override async Task OnTestInitializedAsync()
    {

        // Get container details
        var host = _minioContainer.Hostname;
        var port = _minioContainer.GetMappedPublicPort(9000); // Minio default port

        // Configure Minio client
        var minioClient = new MinioClient()
            .WithEndpoint(host, port)  // Use host and port directly
            .WithCredentials("minioadmin", "minioadmin")
            .WithSSL(false)  // Disable SSL for test container
            .Build();

        // Health check - wait until MinIO is ready
        await WaitForMinioReady(minioClient);

        await EnsureBucketExists(minioClient, _bucketName);

        var settings = Options.Create(new MinioSettings
        {
            BucketName = _bucketName,
            // PublicBaseUrl = _minioContainer.GetConnectionString()
            PublicBaseUrl = $"http://{host}:{port}"
        });

        _service = new MinioImageStorageService(minioClient, settings);
    }



    public new Task DisposeAsync()
    {
        return base.DisposeAsync();
    }

    [Fact]
    public async Task GenerateUploadUrl_ReturnsValidUrl()
    {
        // Arrange
        var objectKey = "test.jpg";

        // Act
        var url = await _service.GenerateUploadUrlAsync(objectKey);

        // Assert
        url.Should().NotBeNullOrWhiteSpace();
        url.Should().Contain(".jpg");
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenObjectMissing()
    {
        // Use GUID to ensure unique non-existent key
        var objectKey = $"nonexistent-{Guid.NewGuid()}.jpg";
        
        var exists = await _service.ExistsAsync(objectKey, CancellationToken.None, _bucketName);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task FullLifecycle_UploadCheckDelete_Succeeds()
    {
        // Arrange
        var objectKey = $"lifecycle-{Guid.NewGuid()}.jpg";
        var uploadUrl = await _service.GenerateUploadUrlAsync(objectKey, _bucketName);

        // Act 1: Upload test object
        using var httpClient = new HttpClient();

        var content = new ByteArrayContent(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }); // PNG header
        System.Net.Http.HttpResponseMessage? response = null;

        const int maxRetries = 3;
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                response = await httpClient.PutAsync(uploadUrl, content);
                response.EnsureSuccessStatusCode();
                break;
            }
            catch (HttpRequestException ex) when (
                ex.StatusCode == HttpStatusCode.ServiceUnavailable && 
                attempt < maxRetries)
            {
                await Task.Delay(1000 * attempt);
            }
        }

        // Assert 1
        response?.EnsureSuccessStatusCode();

        // Act 2: Verify exists
        var exists = await _service.ExistsAsync(objectKey);

        // Assert 2
        exists.Should().BeTrue();

        // Act 3: Delete
        await _service.DeleteAsync(objectKey);

        // Assert 3
        var existsAfterDelete = await _service.ExistsAsync(objectKey);
        existsAfterDelete.Should().BeFalse();
    }

    [Fact]
    public void GetPublicUrl_ReturnsCorrectUrl()
    {
        // Arrange
        var objectKey = "public/test.jpg";
        var host = _minioContainer.Hostname;
        var port = _minioContainer.GetMappedPublicPort(9000);
        var expectedUrl = $"http://{host}:{port}/{objectKey}";

        // Act
        var url = _service.GetPublicUrl(objectKey);

        // Assert
        url.Should().Be(expectedUrl);
    }

    [Fact]
    public void GenerateUserBookObjectKey_ReturnsCorrectPath()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();

        // Act
        var key = _service.GenerateUserBookObjectKey(userId, bookId, "photo.png");

        // Assert
        key.Should().StartWith($"userbooks/{userId}/{bookId}/");
        key.Should().EndWith(".png");
    }
    


    private async Task WaitForMinioReady(IMinioClient minioClient, int maxRetries = 15, int delayMs = 500)
    {
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                // Test bucket operations
                var testBucket = "healthcheck-" + Guid.NewGuid().ToString("N");
                var testObject = "test-object.dat";
                
                // Create and delete test bucket
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(testBucket));
                await minioClient.RemoveBucketAsync(new RemoveBucketArgs().WithBucket(testBucket));
                
                // Test object operations in main bucket
                using var stream = new MemoryStream(new byte[1]);
                await minioClient.PutObjectAsync(
                    new PutObjectArgs()
                        .WithBucket(_bucketName)
                        .WithObject(testObject)
                        .WithStreamData(stream)
                        .WithObjectSize(1)
                );
                
                await minioClient.RemoveObjectAsync(
                    new RemoveObjectArgs()
                        .WithBucket(_bucketName)
                        .WithObject(testObject)
                );
                return;
            }
            catch
            {
                if (attempt == maxRetries)
                    throw new Exception($"MinIO did not become fully ready after {maxRetries} attempts");
                
                await Task.Delay(delayMs);
            }
        }
    }

    private async Task EnsureBucketExists(IMinioClient minioClient, string bucketName)
    {
        var existsArgs = new BucketExistsArgs().WithBucket(bucketName);
        if (!await minioClient.BucketExistsAsync(existsArgs))
        {
            await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }
    }
}
