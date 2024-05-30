using Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.Contracts.Contracts;

namespace Services.Services;

public class ImageService : IImageService
{
    private readonly IConfiguration _config;
    private readonly ILoggerService _logger;

    private static string[]? _validExtensions;

    public ImageService(IConfiguration config, ILoggerService logger)
    {
        _config = config;
        _logger = logger;
        
        _validExtensions ??= config["Image:ValidExtensions"].Split(",");
    }

    public Task<byte[]> GetImage(string url)
    {
        var  split = url.Split(".");
        
        var suffix = split[^1];
        var prefix = string.Concat(split[..^1]);
        
        url = $"{prefix}.{suffix}";
        
        var path = $"{_config["Image:ImageFolder"]}/{url}";
        
        if (File.Exists(path) == false) throw new NotFound("Image not found");
        
        return File.ReadAllBytesAsync(path);
    }

    public async Task<string> CreateImage(string originalName, byte[] imageBytes)
    {
        var extension = originalName.Split(".")[^1];
        var url = $"{Guid.NewGuid()}.{extension}";
        
        //check if the extension is valid
        if (new[] {"jpg", "jpeg", "png", "webp", "gif"}.Contains(extension) == false) 
            throw new BadRequest("Invalid image extension");
        
        var path = $"{_config["Image:ImageFolder"]}/{url}";
        var size = imageBytes.Length;
        if (size > int.Parse(_config["Image:MaxImageSize"])) throw new BadRequest("Image size is too big");
        
        await File.WriteAllBytesAsync(path, imageBytes);
        
        return url;
    }

    public async Task<string?> CreateImage(IFormFile? file, string? hostValue = null)
    {
        if (file == null) return null;
        
        var imageBytes = new byte[file.Length];
        var res = await file.OpenReadStream().ReadAsync(imageBytes);
        var fileName = file.FileName;
        if (res == 0) throw new BadRequest("File is empty");

        //$"https://{request.Host.Value}/image/{await CreateImage(fileName, imageBytes)}";
        return hostValue == null ? await CreateImage(fileName, imageBytes) : 
            $"https://{hostValue}/image/{await CreateImage(fileName, imageBytes)}";
    }

    public void DeleteImage(string url)
    {
        var  split = url.Split(".");
        
        var suffix = split[^1];
        var prefix = string.Concat(split[..^1]);
        
        url = $"{prefix}.{suffix}";

        var path = $"{_config["Image:ImageFolder"]}/{url}";
        File.Delete(path);
    }
}