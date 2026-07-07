using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ElectroShop.Application.Common.Options;
using ElectroShop.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectroShop.Application.Services;

public class CloudinaryImageStorageService : IImageStorageService
{
    private const int MaxFileSize = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private readonly Cloudinary _cloudinary;
    private readonly CloudinarySettings _settings;
    private readonly ILogger<CloudinaryImageStorageService> _logger;

    public CloudinaryImageStorageService(
        IOptions<CloudinarySettings> settings,
        ILogger<CloudinaryImageStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _cloudinary = CreateCloudinaryAccount(_settings);
    }

    public async Task<ImageUploadResultDto> UploadAsync(
        IFormFile file,
        string? folder = null,
        Guid? imageId = null,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(file);
        EnsureConfigured();

        var resolvedImageId = imageId ?? Guid.NewGuid();
        var uploadFolder = string.IsNullOrWhiteSpace(folder) ? _settings.Folder : folder.Trim().Trim('/');
        var publicId = $"{uploadFolder}/{resolvedImageId}";

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = publicId,
            Overwrite = false,
            UniqueFilename = false
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            _logger.LogError(
                "Cloudinary upload failed. PublicId: {PublicId}, Error: {Error}",
                publicId,
                uploadResult.Error.Message);

            throw new InvalidOperationException("Şəkil Cloudinary-ə yüklənərkən xəta baş verdi.");
        }

        _logger.LogInformation(
            "Image uploaded to Cloudinary. PublicId: {PublicId}, Size: {Size} bytes",
            uploadResult.PublicId,
            file.Length);

        return new ImageUploadResultDto
        {
            Url = uploadResult.Url?.ToString(),
            SecureUrl = uploadResult.SecureUrl?.ToString(),
            PublicId = uploadResult.PublicId ?? publicId,
            FileName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Length,
            StorageProvider = "Cloudinary"
        };
    }

    public async Task<bool> DeleteAsync(
        string publicId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return false;

        EnsureConfigured();

        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };

        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Result is "ok" or "not found")
        {
            _logger.LogInformation(
                "Cloudinary image deleted. PublicId: {PublicId}, Result: {Result}",
                publicId,
                result.Result);
            return true;
        }

        _logger.LogWarning(
            "Cloudinary delete returned unexpected result. PublicId: {PublicId}, Result: {Result}",
            publicId,
            result.Result);

        return false;
    }

    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Şəkil faylı boş ola bilməz");

        if (file.Length > MaxFileSize)
            throw new ArgumentException($"Şəkil ölçüsü maksimum {MaxFileSize / (1024 * 1024)} MB ola bilər");

        if (!AllowedContentTypes.Contains(file.ContentType))
            throw new ArgumentException("İcazə verilən formatlar: JPEG, PNG, WebP, GIF");
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_settings.CloudName) ||
            string.IsNullOrWhiteSpace(_settings.ApiKey) ||
            string.IsNullOrWhiteSpace(_settings.ApiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary konfiqurasiyası tam deyil. Cloudinary__CloudName, Cloudinary__ApiKey və Cloudinary__ApiSecret təyin edilməlidir.");
        }
    }

    private static Cloudinary CreateCloudinaryAccount(CloudinarySettings settings)
    {
        var account = new Account(
            settings.CloudName,
            settings.ApiKey,
            settings.ApiSecret);

        return new Cloudinary(account);
    }
}
