namespace ElectroShop.Application.Common.Results;

/// <summary>
/// Static class containing common domain errors
/// Organize your domain-specific errors here
/// </summary>
public static class DomainErrors
{
    public static class General
    {
        public static Error ServerError => Error.Failure(
            "General.ServerError",
            "Server xətası baş verdi. Zəhmət olmasa yenidən cəhd edin.");

        public static Error UnexpectedError => Error.Failure(
            "General.UnexpectedError",
            "Gözlənilməz xəta baş verdi.");

        public static Error ValueIsRequired => Error.Validation(
            "General.ValueIsRequired",
            "Dəyər tələb olunur.");

        public static Error InvalidLength(string name, int maxLength) => Error.Validation(
            "General.InvalidLength",
            $"{name} maksimum {maxLength} simvol ola bilər.");
    }

    public static class Product
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "Product.NotFound",
            $"ID-si {id} olan məhsul tapılmadı.");

        public static Error AlreadyExists(string sku) => Error.Conflict(
            "Product.AlreadyExists",
            $"SKU-su {sku} olan məhsul artıq mövcuddur.");

        public static Error InvalidPrice => Error.Validation(
            "Product.InvalidPrice",
            "Məhsulun qiyməti 0-dan böyük olmalıdır.");

        public static Error InvalidSku => Error.Validation(
            "Product.InvalidSku",
            "Yanlış SKU formatı.");

        public static Error OutOfStock => Error.Failure(
            "Product.OutOfStock",
            "Məhsul stokda yoxdur.");
    }

    public static class ProductImage
    {
        public static Error NotFound(Guid productId, Guid imageId) => Error.NotFound(
            "ProductImage.NotFound",
            $"ID-si {productId} olan məhsulda {imageId} şəkli tapılmadı.");
    }

    public static class ProductVariant
    {
        public static Error AttributeAlreadyExists(string type) => Error.Conflict(
            "ProductVariant.AttributeAlreadyExists",
            $"{type} attribute artıq bu kateqoriyada mövcuddur");

        public static Error ValueAlreadyExists(string value, string type) => Error.Conflict(
            "ProductVariant.ValueAlreadyExists",
            $"{value} dəyəri {type} attribute-u üçün artıq mövcuddur");

        public static Error AttributeDuplicateConstraint => Error.Conflict(
            "ProductVariant.AttributeDuplicateConstraint",
            "Bu attribute artıq kateqoriyada mövcuddur");

        public static Error ValueDuplicateConstraint => Error.Conflict(
            "ProductVariant.ValueDuplicateConstraint",
            "Bu value artıq attribute üçün mövcuddur");

        public static Error DuplicateCombination => Error.Conflict(
            "ProductVariant.DuplicateCombination",
            "Bu variant kombinasiyası artıq məhsulda mövcuddur");

        public static Error RequiredAttributeMissing(string type) => Error.Validation(
            "ProductVariant.RequiredAttributeMissing",
            $"{type} required attribute-dur və variantda göndərilməyib");

        public static Error EmptyAttributes => Error.Validation(
            "ProductVariant.EmptyAttributes",
            "Variant attribute-ları boş ola bilməz");

        public static Error AttributeNotFound(string type) => Error.Validation(
            "ProductVariant.AttributeNotFound",
            $"{type} attribute bu kateqoriyada mövcud deyil");

        public static Error ValueNotFound(string value, string type) => Error.Validation(
            "ProductVariant.ValueNotFound",
            $"{value} dəyəri {type} attribute-u üçün mövcud deyil");

        public static Error CategoryChangeIncompatible => Error.Validation(
            "ProductVariant.CategoryChangeIncompatible",
            "Məhsulun kateqoriyası dəyişdirildiyi üçün mövcud variant attribute-ları uyğun deyil");
    }

    public static class Category
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "Category.NotFound",
            $"ID-si {id} olan kateqoriya tapılmadı.");

        public static Error AlreadyExists(string name) => Error.Conflict(
            "Category.AlreadyExists",
            $"{name} adlı kateqoriya artıq mövcuddur.");
    }

    public static class Brand
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "Brand.NotFound",
            $"ID-si {id} olan brend tapılmadı.");

        public static Error AlreadyExists(string name) => Error.Conflict(
            "Brand.AlreadyExists",
            $"{name} adlı brend artıq mövcuddur.");
    }

    public static class Order
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "Order.NotFound",
            $"ID-si {id} olan sifariş tapılmadı.");

        public static Error EmptyOrder => Error.Validation(
            "Order.EmptyOrder",
            "Sifariş ən azı bir məhsul daxil etməlidir.");

        public static Error InvalidStatus => Error.Validation(
            "Order.InvalidStatus",
            "Yanlış sifariş statusu.");

        public static Error CannotCancel => Error.Failure(
            "Order.CannotCancel",
            "Bu sifariş ləğv edilə bilməz.");

        public static Error OrderDetailNull => Error.Failure(
            "Order.OrderDetailNull",
            "Sifariş detalı tapılmadı.");
    }

    public static class Customer
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "Customer.NotFound",
            $"ID-si {id} olan müştəri tapılmadı.");

        public static Error EmailAlreadyExists(string email) => Error.Conflict(
            "Customer.EmailAlreadyExists",
            $"{email} e-poçt ünvanı artıq istifadə olunur.");

        public static Error InvalidEmail => Error.Validation(
            "Customer.InvalidEmail",
            "Yanlış e-poçt ünvanı.");
    }

    public static class Sale
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            "Sale.NotFound",
            $"ID-si {id} olan satış tapılmadı.");
    }

    public static class ProductRating
    {
        public static Error NotFound(Guid productId) => Error.NotFound(
            "ProductRating.NotFound",
            $"Bu məhsul üçün reytinq tapılmadı.");

        public static Error AlreadyExists(Guid productId) => Error.Conflict(
            "ProductRating.AlreadyExists",
            "Bu məhsul üçün artıq reytinq vermisiniz. Yeniləmək üçün PUT /api/Products/{productId}/ratings/me istifadə edin.");

        public static Error InvalidRatingValue => Error.Validation(
            "ProductRating.InvalidRatingValue",
            "Reytinq 1 ilə 5 arasında olmalıdır.");
    }

    public static class Authentication
    {
        public static Error InvalidCredentials => Error.Unauthorized(
            "Authentication.InvalidCredentials",
            "İstifadəçi adı və ya şifrə səhvdir.");

        public static Error Unauthorized => Error.Unauthorized(
            "Authentication.Unauthorized",
            "Bu əməliyyat üçün icazəniz yoxdur.");

        public static Error TokenExpired => Error.Unauthorized(
            "Authentication.TokenExpired",
            "Sessiya müddətiniz bitib. Zəhmət olmasa yenidən daxil olun.");

        public static Error Forbidden => Error.Forbidden(
            "Authentication.Forbidden",
            "Bu resursa giriş icazəniz yoxdur.");
    }

    public static class Validation
    {
        public static Error Required(string fieldName) => Error.Validation(
            "Validation.Required",
            $"{fieldName} sahəsi məcburidir.");

        public static Error MaxLength(string fieldName, int maxLength) => Error.Validation(
            "Validation.MaxLength",
            $"{fieldName} maksimum {maxLength} simvol ola bilər.");

        public static Error MinLength(string fieldName, int minLength) => Error.Validation(
            "Validation.MinLength",
            $"{fieldName} minimum {minLength} simvol olmalıdır.");

        public static Error InvalidFormat(string fieldName) => Error.Validation(
            "Validation.InvalidFormat",
            $"{fieldName} formatı yanlışdır.");

        public static Error MustBeGreaterThan(string fieldName, decimal value) => Error.Validation(
            "Validation.MustBeGreaterThan",
            $"{fieldName} {value} dəyərindən böyük olmalıdır.");

        public static Error MustBeLessThan(string fieldName, decimal value) => Error.Validation(
            "Validation.MustBeLessThan",
            $"{fieldName} {value} dəyərindən kiçik olmalıdır.");
    }
}

