# PowerShell script for updating database
param(
    [string]$ProjectPath = "src/ElectroShop.Persistence",
    [string]$StartupProject = "src/ElectroShop.WebApi"
)

$contextProject = Join-Path $PSScriptRoot ".." $ProjectPath
$startupProjectPath = Join-Path $PSScriptRoot ".." $StartupProject
$contextPath = "ElectroShop.Persistence.Contexts.ElectroShopDbContext"

Write-Host "Updating database..." -ForegroundColor Green

Set-Location (Join-Path $PSScriptRoot "..")

dotnet ef database update `
    --project $contextProject `
    --startup-project $startupProjectPath `
    --context $contextPath

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database updated successfully!" -ForegroundColor Green
} else {
    Write-Host "Failed to update database!" -ForegroundColor Red
    exit 1
}

