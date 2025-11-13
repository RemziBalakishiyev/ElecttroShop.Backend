# PowerShell script for creating EF Core migrations
param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName,
    
    [string]$ProjectPath = "src/ElectroShop.Persistence"
)

$contextProject = Join-Path $PSScriptRoot ".." $ProjectPath
$contextPath = "ElectroShop.Persistence.Contexts.ElectroShopDbContext"
$startupProject = Join-Path $PSScriptRoot ".." "src/ElectroShop.WebApi"

Write-Host "Creating migration: $MigrationName" -ForegroundColor Green
Write-Host "Context project: $contextProject" -ForegroundColor Cyan
Write-Host "Startup project: $startupProject" -ForegroundColor Cyan

Set-Location (Join-Path $PSScriptRoot "..")

dotnet ef migrations add $MigrationName `
    --project $contextProject `
    --startup-project $startupProject `
    --context $contextPath

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration '$MigrationName' created successfully!" -ForegroundColor Green
} else {
    Write-Host "Failed to create migration!" -ForegroundColor Red
    exit 1
}

