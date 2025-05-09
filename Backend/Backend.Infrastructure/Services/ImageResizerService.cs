using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Infrastructure.Configuration;
using FluentResults;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Backend.Infrastructure.Services;

public class ImageResizerService : IImageResizerService
{
    private readonly IMinioClient _client;
    private readonly MinioSettings _settings;

    public ImageResizerService(IMinioClient minio, IOptions<MinioSettings> options)
    {
        _client = minio;
        _settings = options.Value;
    }

    public async Task<Result<string>> GenerateThumbnailAsync(string originalKey, ThumbnailType type)
    {
        var (width, height) = (type.Width, type.Height);

        using var inStream  = new MemoryStream();

        await _client.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(originalKey)
            .WithCallbackStream(s => s.CopyTo(inStream)));
        inStream.Position = 0;

        // resize
        using var image = Image.Load(inStream);
        image.Mutate(x => x.Resize(new ResizeOptions {
            Mode = ResizeMode.Max,
            Size = new Size(width, height)
        }));

        // save to JPEG stream
        using var outStream = new MemoryStream();
        image.SaveAsJpeg(outStream);
        outStream.Position = 0;

        // upload thumbnail alongside original
        var thumbKey = Path.ChangeExtension(originalKey, null) + "-thumb.jpg";
        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(thumbKey)
            .WithObjectSize(outStream.Length)
            .WithStreamData(outStream)
            .WithContentType("image/jpeg"));
        
        return Result.Ok(thumbKey);
    }
}
