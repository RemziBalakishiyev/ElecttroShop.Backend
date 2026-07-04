namespace ElectroShop.Application.Services;

public record ImageServeResult(
    Stream Stream,
    string ContentType,
    string PhysicalPath,
    ProductImageReference? DbRecord);

public record ProductImageReference(
    Guid ProductImageId,
    Guid ProductId,
    Guid ImageId,
    string ExpectedFileName);

public interface IImageServeService
{
    Task<ImageServeResult?> TryGetImageAsync(
        Guid imageId,
        CancellationToken cancellationToken = default);
}
