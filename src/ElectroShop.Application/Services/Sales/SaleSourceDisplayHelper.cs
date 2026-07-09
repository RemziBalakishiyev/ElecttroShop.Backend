using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.Services.Sales;

internal static class SaleSourceDisplayHelper
{
  public static string ToDisplayName(SaleSource saleSource) => saleSource switch
  {
    SaleSource.ExistingProduct => "Mövcud məhsul",
    SaleSource.ManualEntry => "Manual giriş",
    _ => saleSource.ToString()
  };
}
