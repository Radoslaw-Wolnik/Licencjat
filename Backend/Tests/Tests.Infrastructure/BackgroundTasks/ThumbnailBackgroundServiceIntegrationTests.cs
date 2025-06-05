using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Events;
using Backend.Domain.Enums;
using Backend.Infrastructure.BackgroundTasks;
using Backend.Infrastructure.Configuration;
using Backend.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace Tests.Infrastructure.BackgroundTasks
{
    public class ThumbnailBackgroundServiceIntegrationTests : TestContainersBase, IAsyncLifetime
    {
        private Channel<ThumbnailRequest> _channel = null!;
        private MinioImageStorageService _storage = null!;
        private ImageResizerService _resizer = null!;
        private IMinioClient _minioClient = null!;
        private string _bucketName = "test-bucket";

        public new async Task InitializeAsync()
        {
            // Start the base container (creates and starts the MinIO container, etc.)
            await base.InitializeAsync();

            // Grab host/port from TestContainersBase
            var host = _minioContainer.Hostname;
            var port = _minioContainer.GetMappedPublicPort(9000);

            // Build a MinioClient against the test container (no SSL)
            _minioClient = new MinioClient()
                .WithEndpoint(host, port)
                .WithCredentials("minioadmin", "minioadmin")
                .WithSSL(false)
                .Build();

            // Wait until MinIO is fully ready
            await WaitForMinioReady(_minioClient);

            // Ensure our bucket exists
            await EnsureBucketExists(_minioClient, _bucketName);

            // Create a MinioSettings object (for both storage and resizer)
            var settings = Options.Create(new MinioSettings
            {
                BucketName = _bucketName,
                PublicBaseUrl = $"http://{host}:{port}"
            });

            _storage = new MinioImageStorageService(_minioClient, settings);
            _resizer = new ImageResizerService(_minioClient, settings);
            _channel = Channel.CreateUnbounded<ThumbnailRequest>();
        }

        public new Task DisposeAsync()
        {
            return base.DisposeAsync();
        }

        [Fact]
        public async Task ExecuteAsync_ProcessesRequest_CreatesThumbnail()
        {
            // Arrange: upload one test image under “background-test.jpg”
            var objectKey = $"background-test-{Guid.NewGuid()}.jpg";
            await UploadTestImage(objectKey);

            // Create and start the background service
            var logger = new LoggerFactory().CreateLogger<ThumbnailBackgroundService>();
            var service = new ThumbnailBackgroundService(_channel, _resizer, _storage, logger);
            var serviceTask = service.StartAsync(CancellationToken.None);

            // Push one ThumbnailRequest onto the channel
            await _channel.Writer.WriteAsync(new ThumbnailRequest(objectKey, ThumbnailType.Cover));

            // Give it a little time to pick up the request and execute
            // In practice, you might wait on a more deterministic signal (e.g. 
            // the channel’s completion) or poll for existence. But a brief delay is simpler here.
            await Task.Delay(3000);

            // Stop the service gracefully
            await service.StopAsync(CancellationToken.None);
            await serviceTask;

            // Compute the expected thumbnail key (same logic as ImageResizerService.GenerateThumbnailAsync uses):
            var expectedThumbKey = Path.ChangeExtension(objectKey, null) + "-thumb.jpg";

            // Assert: the thumbnail must now exist in MinIO
            var exists = await _storage.ExistsAsync(expectedThumbKey, CancellationToken.None, _bucketName);
            exists.Should().BeTrue($"the background worker should have created “{expectedThumbKey}” after seeing the request");
        }

        private async Task UploadTestImage(string objectKey)
        {
            // Create a 600×800 JPEG in memory
            using var image = new Image<Rgba32>(600, 800);
            using var ms = new MemoryStream();
            image.SaveAsJpeg(ms);
            ms.Position = 0;

            // Upload it directly via MinIO SDK (PutObjectAsync)
            var putArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectKey)
                .WithStreamData(ms)
                .WithObjectSize(ms.Length)
                .WithContentType("image/jpeg");
            await _minioClient.PutObjectAsync(putArgs);
        }

        private async Task WaitForMinioReady(IMinioClient client, int maxRetries = 15, int delayMs = 500)
        {
            for (var attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Create & delete a throwaway bucket
                    var healthBucket = $"healthcheck-{Guid.NewGuid():N}";
                    await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(healthBucket));
                    await client.RemoveBucketAsync(new RemoveBucketArgs().WithBucket(healthBucket));
                    return;
                }
                catch
                {
                    if (attempt == maxRetries)
                        throw new Exception($"MinIO did not become ready after {maxRetries} attempts");
                    await Task.Delay(delayMs);
                }
            }
        }

        private async Task EnsureBucketExists(IMinioClient client, string bucket)
        {
            var existsArgs = new BucketExistsArgs().WithBucket(bucket);
            if (!await client.BucketExistsAsync(existsArgs))
            {
                await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket));
            }
        }
    }
}
