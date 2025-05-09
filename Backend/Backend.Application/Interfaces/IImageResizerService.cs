using Backend.Domain.Common;
using FluentResults;

namespace Backend.Application.Interfaces;

public interface IImageResizerService
{
    Task<Result<string>> GenerateThumbnailAsync(string originalKey, ThumbnailType thumbnailType);
}
