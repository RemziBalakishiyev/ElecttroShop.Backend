# Lokal test mühiti — PostgreSQL + API
# Istifade: .\scripts\start-local-dev.ps1

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $PSScriptRoot

function Test-PortInUse {
    param([int]$Port)
    return [bool](Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue)
}

Push-Location $Root
try {
    & (Join-Path $PSScriptRoot "stop-local-dev.ps1")

    Write-Host "==> PostgreSQL (Docker) yoxlanilir..." -ForegroundColor Cyan
    $dbRunning = docker ps --filter "name=electroshop-db" --filter "status=running" -q
    if ($dbRunning) {
        Write-Host "    electroshop-db artiq isleyir." -ForegroundColor Green
    }
    elseif (Test-PortInUse -Port 5434) {
        Write-Host "    Port 5434 artiq istifade olunur (basqa PostgreSQL). Docker DB kecilir." -ForegroundColor Yellow
    }
    else {
        docker compose up -d
        if ($LASTEXITCODE -ne 0) {
            docker-compose up -d
        }

        Write-Host "==> DB hazir olana qeder gozlenilir..." -ForegroundColor Cyan
        for ($i = 0; $i -lt 30; $i++) {
            $status = docker inspect -f "{{.State.Health.Status}}" electroshop-db 2>$null
            if ($status -eq "healthy") { break }
            Start-Sleep -Seconds 2
        }
    }

    $devSettings = Join-Path $Root "src\ElectroShop.WebApi\appsettings.Development.json"
    $devExample = Join-Path $Root "src\ElectroShop.WebApi\appsettings.Development.example.json"
    if (-not (Test-Path $devSettings)) {
        Write-Host "==> appsettings.Development.json yaradilir..." -ForegroundColor Yellow
        Copy-Item $devExample $devSettings
    }

    Write-Host ""
    Write-Host "==> API basladilir: http://localhost:5223/swagger" -ForegroundColor Green
    Write-Host "    Admin: admin@electroshop.az / Admin123!" -ForegroundColor Green
    Write-Host ""

    dotnet run --project (Join-Path $Root "src\ElectroShop.WebApi\ElectroShop.WebApi.csproj") --launch-profile http
}
finally {
    Pop-Location
}
