using ClosedXML.Excel;
using ElectroShop.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ElectroShop.Application.Services.Sales;

public class SalesExportService : ISalesExportService
{
    private const string CurrencyExcelFormat = "#,##0.00 \"₼\"";
    private const string DateFormat = "dd.MM.yyyy";
    private static readonly XLColor ProfitPositiveColor = XLColor.FromHtml("#008000");
    private static readonly XLColor ProfitNegativeColor = XLColor.FromHtml("#FF0000");

    static SalesExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateExcel(MonthlySalesReportDto report)
    {
        using var workbook = new XLWorkbook();
        var sheetName = SalesMonthHelper.GetSheetName(report.Year, report.Month);
        if (sheetName.Length > 31)
            sheetName = sheetName[..31];

        var ws = workbook.Worksheets.Add(sheetName);
        var title = SalesMonthHelper.GetReportTitle(report.Month);

        ws.Cell(1, 1).Value = title;
        ws.Range(1, 1, 1, 11).Merge();
        ws.Cell(1, 1).Style.Font.Bold = true;
        ws.Cell(1, 1).Style.Font.FontSize = 14;

        ws.Cell(2, 1).Value = $"Hesabat tarixi: {report.ReportDate.ToString(DateFormat)}";
        ws.Range(2, 1, 2, 11).Merge();

        var summaryStartRow = 4;
        WriteSummarySection(ws, report, summaryStartRow);

        var tableHeaderRow = summaryStartRow + 10;
        WriteTableHeaders(ws, tableHeaderRow);

        var dataStartRow = tableHeaderRow + 1;
        var currentRow = dataStartRow;
        foreach (var item in report.Items)
        {
            WriteTableRow(ws, currentRow, item);
            currentRow++;
        }

        var lastDataRow = Math.Max(currentRow - 1, tableHeaderRow);
        ApplyTableFormatting(ws, tableHeaderRow, lastDataRow);
        ws.Columns(1, 11).AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GeneratePdf(MonthlySalesReportDto report)
    {
        var title = SalesMonthHelper.GetReportTitle(report.Month);
        var summary = report.Summary;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Column(column =>
                {
                    column.Item().Text(title).Bold().FontSize(14);
                    column.Item().PaddingTop(4).Text($"Hesabat tarixi: {report.ReportDate.ToString(DateFormat)}")
                        .FontSize(9);
                });

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Item().Element(c => WritePdfSummary(c, summary));
                    column.Item().PaddingTop(12).Text("Satılan məhsullar").Bold().FontSize(10);
                    column.Item().PaddingTop(6).Element(c => WritePdfTable(c, report.Items));
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Səhifə ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    private static void WriteSummarySection(IXLWorksheet ws, MonthlySalesReportDto report, int startRow)
    {
        var summary = report.Summary;
        var rows = new (string Label, object Value, bool IsCurrency, bool IsProfit)[]
        {
            ("Hesabat ayı", $"{report.MonthName} {report.Year}", false, false),
            ("Satış sayı", summary.SalesCount, false, false),
            ("Satılan ümumi məhsul miqdarı", summary.TotalQuantity, false, false),
            ("Ümumi satış məbləği", summary.TotalSalesAmount, true, false),
            ("Ümumi maya dəyəri", summary.TotalCostAmount, true, false),
            ("Ümumi xərclər", summary.TotalExpenses, true, false),
            ("Ümumi mənfəət", summary.GrossProfit, true, true),
            ("Xalis gəlir", summary.NetProfit, true, true)
        };

        var row = startRow;
        foreach (var (label, value, isCurrency, isProfit) in rows)
        {
            ws.Cell(row, 1).Value = label;
            ws.Cell(row, 1).Style.Font.Bold = true;
            var valueCell = ws.Cell(row, 2);
            valueCell.Value = XLCellValue.FromObject(value);

            if (isCurrency)
            {
                valueCell.Style.NumberFormat.Format = CurrencyExcelFormat;
                if (isProfit)
                    ApplyProfitStyle(valueCell, Convert.ToDecimal(value));
            }

            row++;
        }
    }

    private static void WriteTableHeaders(IXLWorksheet ws, int row)
    {
        var headers = new[]
        {
            "Məhsul adı",
            "SKU / Məhsul kodu",
            "Kateqoriya",
            "Satış növü",
            "Satış qiyməti",
            "Miqdar",
            "Ümumi maya dəyəri",
            "Ümumi satış məbləği",
            "Ümumi xərclər",
            "Mənfəət",
            "Satış tarixi"
        };

        for (var col = 0; col < headers.Length; col++)
        {
            var cell = ws.Cell(row, col + 1);
            cell.Value = headers[col];
            cell.Style.Font.Bold = true;
        }
    }

    private static void WriteTableRow(IXLWorksheet ws, int row, MonthlySalesReportItemDto item)
    {
        ws.Cell(row, 1).Value = item.ProductName;
        ws.Cell(row, 2).Value = item.Sku ?? item.ProductCode ?? string.Empty;
        ws.Cell(row, 3).Value = item.CategoryName ?? string.Empty;
        ws.Cell(row, 4).Value = item.SaleType;
        ws.Cell(row, 5).Value = item.SalePrice;
        ws.Cell(row, 6).Value = item.Quantity;
        ws.Cell(row, 7).Value = item.TotalCostAmount;
        ws.Cell(row, 8).Value = item.TotalSalesAmount;
        ws.Cell(row, 9).Value = item.TotalExpenses;

        var profitCell = ws.Cell(row, 10);
        profitCell.Value = item.Profit;
        ApplyProfitStyle(profitCell, item.Profit);

        ws.Cell(row, 11).Value = item.SaleDate;
        ws.Cell(row, 11).Style.DateFormat.Format = DateFormat;
    }

    private static void ApplyTableFormatting(IXLWorksheet ws, int headerRow, int lastDataRow)
    {
        var currencyColumns = new[] { 5, 7, 8, 9, 10 };
        foreach (var col in currencyColumns)
        {
            var range = ws.Range(headerRow + 1, col, Math.Max(lastDataRow, headerRow + 1), col);
            range.Style.NumberFormat.Format = CurrencyExcelFormat;
        }

        ws.SheetView.FreezeRows(headerRow);

        if (lastDataRow >= headerRow)
        {
            ws.Range(headerRow, 1, lastDataRow, 11).SetAutoFilter();
        }
    }

    private static void ApplyProfitStyle(IXLCell cell, decimal profit)
    {
        cell.Style.Font.FontColor = profit >= 0 ? ProfitPositiveColor : ProfitNegativeColor;
    }

    private static void WritePdfSummary(IContainer container, MonthlySalesReportSummaryDto summary)
    {
        container.Column(column =>
        {
            column.Spacing(4);
            WritePdfSummaryRow(column, "Ümumi satış məbləği", FormatAzn(summary.TotalSalesAmount));
            WritePdfSummaryRow(column, "Bu ayın xərci", FormatAzn(summary.TotalExpenses));
            WritePdfSummaryRow(column, "Bu ayın qazancı", FormatAzn(summary.GrossProfit), summary.GrossProfit);
            WritePdfSummaryRow(column, "Bu ayın xalis gəliri", FormatAzn(summary.NetProfit), summary.NetProfit);
            WritePdfSummaryRow(column, "Satış sayı", summary.SalesCount.ToString());
            WritePdfSummaryRow(column, "Satılan məhsul miqdarı", summary.TotalQuantity.ToString("N0"));
        });
    }

    private static void WritePdfSummaryRow(
        ColumnDescriptor column,
        string label,
        string value,
        decimal? profitValue = null)
    {
        column.Item().Row(row =>
        {
            row.ConstantItem(140).Text(label).SemiBold();
            var text = row.RelativeItem().Text(value);
            if (profitValue.HasValue)
            {
                var color = profitValue.Value >= 0 ? Colors.Green.Medium : Colors.Red.Medium;
                text.FontColor(color);
            }
        });
    }

    private static void WritePdfTable(IContainer container, List<MonthlySalesReportItemDto> items)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2);
                columns.RelativeColumn(1.2f);
                columns.RelativeColumn(1.2f);
                columns.RelativeColumn(1.2f);
                columns.RelativeColumn(1);
                columns.RelativeColumn(0.7f);
                columns.RelativeColumn(1);
                columns.RelativeColumn(1);
                columns.RelativeColumn(0.9f);
                columns.RelativeColumn(0.9f);
                columns.RelativeColumn(1);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("Məhsul adı").Bold();
                header.Cell().Element(CellStyle).Text("SKU").Bold();
                header.Cell().Element(CellStyle).Text("Kateqoriya").Bold();
                header.Cell().Element(CellStyle).Text("Satış növü").Bold();
                header.Cell().Element(CellStyle).Text("Satış qiyməti").Bold();
                header.Cell().Element(CellStyle).Text("Miqdar").Bold();
                header.Cell().Element(CellStyle).Text("Maya dəyəri").Bold();
                header.Cell().Element(CellStyle).Text("Satış məbləği").Bold();
                header.Cell().Element(CellStyle).Text("Xərclər").Bold();
                header.Cell().Element(CellStyle).Text("Mənfəət").Bold();
                header.Cell().Element(CellStyle).Text("Satış tarixi").Bold();
            });

            foreach (var item in items)
            {
                table.Cell().Element(CellStyle).Text(item.ProductName);
                table.Cell().Element(CellStyle).Text(item.Sku ?? item.ProductCode ?? "-");
                table.Cell().Element(CellStyle).Text(item.CategoryName ?? "-");
                table.Cell().Element(CellStyle).Text(item.SaleType);
                table.Cell().Element(CellStyle).AlignRight().Text(FormatAzn(item.SalePrice));
                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString("N0"));
                table.Cell().Element(CellStyle).AlignRight().Text(FormatAzn(item.TotalCostAmount));
                table.Cell().Element(CellStyle).AlignRight().Text(FormatAzn(item.TotalSalesAmount));
                table.Cell().Element(CellStyle).AlignRight().Text(FormatAzn(item.TotalExpenses));
                table.Cell().Element(CellStyle).AlignRight().Text(FormatAzn(item.Profit))
                    .FontColor(item.Profit >= 0 ? Colors.Green.Medium : Colors.Red.Medium);
                table.Cell().Element(CellStyle).Text(item.SaleDate.ToString(DateFormat));
            }
        });
    }

    private static IContainer CellStyle(IContainer container) =>
        container.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3).PaddingHorizontal(2);

    private static string FormatAzn(decimal amount) =>
        $"{amount:N2} ₼";
}
