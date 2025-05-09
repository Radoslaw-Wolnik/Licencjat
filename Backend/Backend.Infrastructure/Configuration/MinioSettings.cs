namespace Backend.Infrastructure.Configuration;

public class MinioSettings
{
    public string Endpoint       { get; set; } = null!;
    public string AccessKey      { get; set; } = null!;
    public string SecretKey      { get; set; } = null!;
    public string BucketName     { get; set; } = "app-images";
    public string PublicBaseUrl { get; set; } = null!;
    public int    ExpiryMinutes  { get; set; } = 15;
}
