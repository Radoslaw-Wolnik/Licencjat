using Backend.Domain.Common;

namespace Backend.Domain.Events;

public record ThumbnailRequest(string ObjectKey, ThumbnailType Type);
