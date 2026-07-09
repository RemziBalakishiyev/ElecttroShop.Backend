# Backend Change Result

## Summary
Satışlar moduluna ay və il üzrə Excel və PDF hesabat export funksionallığı əlavə edildi. Bütün hesablamalar və fayl generasiyası backend-də həyata keçirilir; frontend yalnız API çağırıb faylı download edə bilər.

## Changed Endpoints

### GET /api/sales/export/excel
- **Method:** GET
- **URL:** `/api/sales/export/excel?year={year}&month={month}`
- **Auth:** JWT Bearer token (`[Authorize]` — mövcud satışlar səhifəsi ilə eyni)
- **Old behavior:** Endpoint mövcud deyildi
- **New behavior:** Seçilmiş təqvim ayı üzrə satış hesabatını Excel faylı kimi qaytarır
- **Request body:** yoxdur
- **Query params:**
  - `year` (required, int) — 2000..2100
  - `month` (required, int) — 1..12
- **Response body:** binary Excel faylı
- **Response headers:**
  - `Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
  - `Content-Disposition: attachment; filename="MAY_AYI_SATIS_2026.xlsx"`
- **Validation rules:**
  - `year` — 2000-2100 arası
  - `month` — 1-12 arası
- **Error responses:**
  - `400` — etibarsız parametr
  - `401` — token yoxdur və ya etibarsızdır

### GET /api/sales/export/pdf
- **Method:** GET
- **URL:** `/api/sales/export/pdf?year={year}&month={month}`
- **Auth:** JWT Bearer token
- **Old behavior:** Endpoint mövcud deyildi
- **New behavior:** Seçilmiş təqvim ayı üzrə satış hesabatını PDF faylı kimi qaytarır
- **Request body:** yoxdur
- **Query params:** eyni (`year`, `month`)
- **Response body:** binary PDF faylı
- **Response headers:**
  - `Content-Type: application/pdf`
  - `Content-Disposition: attachment; filename="MAY_AYI_SATIS_2026.pdf"`
- **Validation rules:** eyni
- **Error responses:** eyni

## Changed Models / DTOs
Yeni internal report DTO-ları (JSON API response deyil, yalnız export generasiyası üçün):
- `MonthlySalesReportDto`
- `MonthlySalesReportSummaryDto`
- `MonthlySalesReportItemDto`
- `SalesExportFileDto`

Mövcud `SaleListItemDto` və digər satış API modelləri dəyişməyib.

## Database / Business Rule Changes
- Migration yoxdur
- Satış filteri: `SoldAt >= ayın1ciGünüUTC && SoldAt < növbətiAyın1ciGünüUTC`
- Summary hesablamaları mövcud `GetSalesStatisticsAsync` aggregation ilə eynidir:
  - `GrossProfit = TotalSalesAmount - TotalCostAmount`
  - `NetProfit = TotalSalesAmount - TotalCostAmount - TotalExpenses`
- Item `Profit` = entity `Profit` (xalis mənfəət)

## Frontend Impact

### Admin
**Tələb olunan dəyişikliklər:**

1. **Satışlar səhifəsinə export düymələri əlavə edin** (Excel və PDF)
   - İstifadəçi ay və il seçir (mövcud filter UI-dan istifadə edə bilərsiniz)
   - Düymə klikində API çağırılıb fayl browser download edilməlidir

2. **Excel export API çağırışı:**
   ```
   GET /api/sales/export/excel?year=2026&month=5
   Authorization: Bearer {token}
   ```

3. **PDF export API çağırışı:**
   ```
   GET /api/sales/export/pdf?year=2026&month=5
   Authorization: Bearer {token}
   ```

4. **Download nümunəsi (TypeScript):**
   ```typescript
   async function downloadSalesExport(
     format: 'excel' | 'pdf',
     year: number,
     month: number,
     token: string
   ): Promise<void> {
     const path = format === 'excel' ? 'excel' : 'pdf';
     const response = await fetch(
       `/api/sales/export/${path}?year=${year}&month=${month}`,
       { headers: { Authorization: `Bearer ${token}` } }
     );
     if (!response.ok) throw new Error('Export uğursuz oldu');

     const blob = await response.blob();
     const disposition = response.headers.get('Content-Disposition');
     const fileNameMatch = disposition?.match(/filename="?([^";\n]+)"?/);
     const fileName = fileNameMatch?.[1] ?? `SATIS_${year}_${month}.${format === 'excel' ? 'xlsx' : 'pdf'}`;

     const url = URL.createObjectURL(blob);
     const a = document.createElement('a');
     a.href = url;
     a.download = fileName;
     a.click();
     URL.revokeObjectURL(url);
   }
   ```

5. **Xəta idarəetməsi:**
   - `400` — "Etibarsız ay və ya il" mesajı
   - `401` — login səhifəsinə yönləndirmə

6. **Qeyd:** Dashboard `GET /api/dashboard/statistics` cari ay üçün month-to-date göstərir; export isə **tam təqvim ayı** üçündür. UI-da bu fərqi nəzərə alın.

### User
No frontend change required.

## OpenAPI
- contracts/openapi.json updated: yes
- contracts/openapi.diff.md updated: yes

## Test Result
- Backend build: Application və Persistence uğurlu (0 error). WebApi build IIS Express file lock səbəbindən lokal mühitdə uğursuz ola bilər — VS/IIS Express dayandırıb yenidən build edin.
- Backend tests: test layihəsi yoxdur
- Manual API test:
  1. Admin token ilə `GET /api/sales/export/excel?year=2026&month=5` — `.xlsx` faylı yüklənməlidir
  2. `GET /api/sales/export/pdf?year=2026&month=5` — `.pdf` faylı yüklənməlidir
  3. `month=0` və ya `year=1999` — `400 BadRequest`
  4. Token olmadan — `401 Unauthorized`
  5. Data olmayan ay — boş hesabat faylı (200 OK)
- Known issues: yoxdur
