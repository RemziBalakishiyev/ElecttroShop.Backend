namespace ElectroShop.Application.Services;

/// <summary>
/// Image upload üçün scoped context service
/// Stream-i handler-a ötürmək üçün istifadə olunur
/// </summary>
public interface IImageUploadContext
{
    Stream? ImageStream { get; set; }
}



