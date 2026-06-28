namespace ElectroShop.Application.Common;

public static class LookupCacheKeys
{
    public const string Categories = "CategoriesLookup";
    public const string CategoriesIncludeAll = "CategoriesLookup_IncludeAll";
    public const string Brands = "BrandsLookup";

    public static string GetCategoriesLookupKey(bool includeAll, Guid? parentId)
    {
        if (parentId.HasValue)
            return $"CategoriesLookup_Parent_{parentId.Value:N}";

        return includeAll ? CategoriesIncludeAll : Categories;
    }
}
