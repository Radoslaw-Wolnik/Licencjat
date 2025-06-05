using Backend.Domain.Common;
using Backend.Infrastructure.Configuration;
using Backend.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Net;


namespace Tests.Infrastructure.Services;

public class ImageResizerServiceIntegrationTests : TestContainersBase, IAsyncLifetime
{
    private ImageResizerService _service = null!;
    private MinioImageStorageService _storage = null!;
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

        _storage = new MinioImageStorageService(minioClient, settings);
        _service = new ImageResizerService(minioClient, settings);
    }

    public new Task DisposeAsync()
    {
        return base.DisposeAsync();
    }


    [Fact]
    public async Task GenerateThumbnailAsync_CreatesValidThumbnail()
    {
        // Arrange
        var objectKey = $"test-image-{Guid.NewGuid()}.jpg";
        var uploadUrl = await _storage.GenerateUploadUrlAsync(objectKey, _bucketName);

        using var image = new Image<Rgba32>(300, 400);
        using var ms = new MemoryStream();
        image.SaveAsJpeg(ms);
        ms.Position = 0;

        // Act 1: Upload test object
        using var httpClient = new HttpClient();

        // Wrap the image‐bytes in StreamContent:
        using var content = new StreamContent(ms);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        
        System.Net.Http.HttpResponseMessage? uploadResponse = null;

        const int maxRetries = 3;
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                uploadResponse = await httpClient.PutAsync(uploadUrl, content);
                uploadResponse.EnsureSuccessStatusCode();
                break;
            }
            catch (HttpRequestException ex) when (
                ex.StatusCode == HttpStatusCode.ServiceUnavailable && 
                attempt < maxRetries)
            {
                await Task.Delay(1000 * attempt);
            }
        }

        uploadResponse?.EnsureSuccessStatusCode();

        // Act 2: sanity‐check that the original exists
        var exists = await _storage.ExistsAsync(objectKey, CancellationToken.None, _bucketName);
        exists.Should().BeTrue("the original object should exist after upload");

        // Act 3: call GenerateThumbnailAsync
        var result = await _service.GenerateThumbnailAsync(objectKey, ThumbnailType.Cover);

        // Assert: the Result should be success, and it should give us a new key
        result.IsSuccess.Should().BeTrue();
        var thumbKey = result.Value;

        // Verify thumbnail now exists
        var thumbnailExists = await _storage.ExistsAsync(thumbKey, CancellationToken.None, _bucketName);
        thumbnailExists.Should().BeTrue("the thumbnail should have been written back to MinIO");

        // Act 4: download the thumbnail bytes from MinIO
        var downloadUrl = await _storage.GenerateSignedDownloadUrlAsync(thumbKey, TimeSpan.FromMinutes(5), _bucketName);
        using var downloadResponse = await httpClient.GetAsync(downloadUrl);
        downloadResponse.EnsureSuccessStatusCode();

        // Read the response content into a MemoryStream so ImageSharp can decode it
        await using var outStream = new MemoryStream();
        await downloadResponse.Content.CopyToAsync(outStream);
        outStream.Position = 0;

        // Assert: load with ImageSharp and check dimensions (Cover => 150px width, aspect preserved)
        using var thumbImage = Image.Load(outStream);
        thumbImage.Width.Should().Be(150, "Cover thumbnails are defined to be 150px wide");
        thumbImage.Height.Should().Be(200, "Cover thumbnails maintain aspect ratio (original was 300x400)");
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