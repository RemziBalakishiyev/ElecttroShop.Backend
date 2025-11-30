namespace ElectroShop.Domain.Enums;

/// <summary>
/// Endirim tipi - endirimin hansı qaynaqdan gəldiyini göstərir
/// </summary>
public enum DiscountType
{
    /// <summary>
    /// Məhsula xüsusi endirim (ən yüksək prioritet)
    /// </summary>
    Product = 0,

    /// <summary>
    /// Brend endirimi
    /// </summary>
    Brand = 1,

    /// <summary>
    /// Kateqoriya endirimi (ən aşağı prioritet)
    /// </summary>
    Category = 2
}



