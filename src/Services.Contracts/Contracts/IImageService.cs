using Microsoft.AspNetCore.Http;

namespace Services.Contracts.Contracts;

public interface IImageService
{
    Task<byte[]> GetImage(string url);
    Task<string> CreateImage(string originalName, byte[] imageBytes);
    Task<string?> CreateImage(IFormFile? file, string? hostValue = null);
    void DeleteImage(string imageName);
}