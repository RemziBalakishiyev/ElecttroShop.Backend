using ElectroShop.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ElectroShop.Application.Services;

public interface IImageStorageService
{
    Task<ImageUploadResultDto> UploadAsync(
        IFormFile file,
        string? folder = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string publicId,
        CancellationToken cancellationToken = default);
}
