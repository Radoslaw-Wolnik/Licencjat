namespace Backend.Domain.Common;

public sealed class ThumbnailType
{
    public static readonly ThumbnailType ProfilePicture = new("ProfilePicture", 40, 40);
    public static readonly ThumbnailType Cover = new("Cover", 150, 200);

    public string Name { get; }
    public int Width { get; }
    public int Height { get; }

    private ThumbnailType(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;
    }

    public override string ToString() => Name;
}
