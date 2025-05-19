using System.Threading.Channels;
using Backend.Application.Interfaces;
using Backend.Domain.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Minio.Exceptions;
using System.Threading.Tasks;
using System.Threading;
namespace Backend.Infrastructure.BackgroundTasks;

public class ThumbnailBackgroundService : BackgroundService
{
    private readonly Channel<ThumbnailRequest>       _channel;
    private readonly IImageResizerService            _resizer;
    private readonly IImageStorageService            _storage;
    private readonly ILogger<ThumbnailBackgroundService> _logger;

    public ThumbnailBackgroundService(
        Channel<ThumbnailRequest> channel,
        IImageResizerService      resizer,
        IImageStorageService      storage,
        ILogger<ThumbnailBackgroundService> logger)
    {
        _channel = channel;
        _resizer = resizer;
        _storage = storage;
        _logger  = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = _channel.Reader;

        while (await reader.WaitToReadAsync(stoppingToken))
        {
            while (reader.TryRead(out var req))
            {
                try
                {
                    // optionally re-check existence
                    if (!await _storage.ExistsAsync(req.ObjectKey, stoppingToken))
                        continue;

                    var result = await _resizer.GenerateThumbnailAsync(
                        req.ObjectKey,
                        req.Type);

                    if (result.IsFailed)
                    {
                        // Domain-level failure (e.g., invalid image format)
                        _logger.LogError(
                            "Thumbnail generation failed for {ObjectKey}: {Errors}",
                            req.ObjectKey,
                            string.Join("; ", result.Errors));
                    }
                }
                catch (MinioException ex)
                {
                    // Transient or network errors with MinIO
                    _logger.LogError(
                        ex,
                        "MinIO error when processing {ObjectKey}, retrying shortly",
                        req.ObjectKey);

                    // simple retry: re-enqueue after delay
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    await _channel.Writer.WriteAsync(req, stoppingToken);
                }
                catch (Exception ex)
                {
                    // Unexpected errors
                    _logger.LogError(
                        ex,
                        "Unexpected error processing thumbnail for {ObjectKey}",
                        req.ObjectKey);
                    // could push to a dead-letter queue or metrics
                }
            }
        }
    }
}
