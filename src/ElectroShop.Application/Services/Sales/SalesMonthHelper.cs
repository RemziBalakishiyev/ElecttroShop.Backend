namespace ElectroShop.Application.Services.Sales;

internal static class SalesMonthHelper
{
  private static readonly string[] DisplayMonthNames =
  [
    "YANVAR", "FEVRAL", "MART", "APREL", "MAY", "İYUN",
    "İYUL", "AVQUST", "SENTYABR", "OKTYABR", "NOYABR", "DEKABR"
  ];

  private static readonly string[] FileSafeMonthNames =
  [
    "YANVAR", "FEVRAL", "MART", "APREL", "MAY", "IYUN",
    "IYUL", "AVQUST", "SENTYABR", "OKTYABR", "NOYABR", "DEKABR"
  ];

  public static string GetDisplayMonthName(int month) =>
    DisplayMonthNames[month - 1];

  public static string GetFileSafeMonthName(int month) =>
    FileSafeMonthNames[month - 1];

  public static string GetSheetName(int year, int month) =>
    $"{GetDisplayMonthName(month)} {year}";

  public static string GetReportTitle(int month) =>
    $"{GetDisplayMonthName(month)} AYI SATIŞ HESABATI";

  public static string GetExportFileName(int year, int month, string extension)
  {
    var ext = extension.TrimStart('.');
    return $"{GetFileSafeMonthName(month)}_AYI_SATIS_{year}.{ext}";
  }

  public static (DateTime StartUtc, DateTime EndUtcExclusive) GetMonthRangeUtc(int year, int month)
  {
    var startUtc = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
    return (startUtc, startUtc.AddMonths(1));
  }
}
