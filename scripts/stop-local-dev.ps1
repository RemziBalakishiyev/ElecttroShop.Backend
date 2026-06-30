# ElectroShop lokal prosesleri dayandir (build lock problemini aradan qaldirir)
# Istifade: .\scripts\stop-local-dev.ps1

$ErrorActionPreference = "SilentlyContinue"

Write-Host "==> Port 5223 istifade eden prosesler dayandirilir..." -ForegroundColor Cyan
$connections = Get-NetTCPConnection -LocalPort 5223 -State Listen
foreach ($connection in $connections) {
    Stop-Process -Id $connection.OwningProcess -Force
}

Write-Host "==> ElectroShop.WebApi prosesleri dayandirilir..." -ForegroundColor Cyan
Get-Process -Name "ElectroShop.WebApi" | Stop-Process -Force

Write-Host "Hazir." -ForegroundColor Green
