namespace Backend.Application.Interfaces;

public interface IImageStorageService
{
    // Generates a pre-signed URL for a user to upload a profile picture.
    Task<string> GenerateProfilePictureUploadUrlAsync(Guid userId, string fileName, TimeSpan expiration);
    
    // Generates a pre-signed URL for a user to upload a book cover image.
    Task<string> GenerateUserBookCoverUploadUrlAsync(Guid userId, string fileName, TimeSpan expiration);

    // Generates a pre-signed URL for downloading an image.
    Task<string> GenerateDownloadUrlAsync(string objectKey, TimeSpan expiration);
}
