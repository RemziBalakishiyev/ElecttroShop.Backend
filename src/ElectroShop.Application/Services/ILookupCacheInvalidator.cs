namespace ElectroShop.Application.Services;

public interface ILookupCacheInvalidator
{
    void InvalidateCategoriesLookup();

    void InvalidateBrandsLookup();
}
