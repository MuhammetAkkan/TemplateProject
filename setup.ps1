# ============================================================================
# Template Projesi Kurulum Scripti
# ============================================================================
# Bu script projeyi temizler, bagimliliklari yukler ve derler.
# Kullanim: .\setup.ps1
# ============================================================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Template Proje Kurulum Basladi" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
$scriptPath = $PSScriptRoot
Set-Location $scriptPath
$solutionFile = Get-ChildItem -Filter "*.sln" | Select-Object -First 1
if (-not $solutionFile) {
    Write-Host "[HATA] Solution (.sln) dosyasi bulunamadi!" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Solution: $($solutionFile.Name)" -ForegroundColor Green
Write-Host ""
Write-Host "[1/4] Proje temizleniyor (dotnet clean)..." -ForegroundColor Yellow
dotnet clean $solutionFile.FullName --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    Write-Host "[HATA] dotnet clean basarisiz oldu!" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Temizleme tamamlandi" -ForegroundColor Green
Write-Host ""
Write-Host "[2/4] bin/ ve obj/ klasorleri siliniyor..." -ForegroundColor Yellow
$binFolders = Get-ChildItem -Path . -Recurse -Directory -Filter "bin" -ErrorAction SilentlyContinue
$objFolders = Get-ChildItem -Path . -Recurse -Directory -Filter "obj" -ErrorAction SilentlyContinue
$totalFolders = $binFolders.Count + $objFolders.Count
if ($totalFolders -gt 0) {
    foreach ($folder in $binFolders) {
        Remove-Item -Path $folder.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
    foreach ($folder in $objFolders) {
        Remove-Item -Path $folder.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }
    Write-Host "[OK] $totalFolders klasor silindi (bin: $($binFolders.Count), obj: $($objFolders.Count))" -ForegroundColor Green
} else {
    Write-Host "[OK] Silinecek bin/obj klasoru bulunamadi" -ForegroundColor Green
}
Write-Host ""
Write-Host "[3/4] Bagimliliklar yukleniyor (dotnet restore)..." -ForegroundColor Yellow
dotnet restore $solutionFile.FullName --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    Write-Host "[HATA] dotnet restore basarisiz oldu!" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Bagimliliklar yuklendi" -ForegroundColor Green
Write-Host ""
Write-Host "[4/4] Proje derleniyor (dotnet build)..." -ForegroundColor Yellow
dotnet build $solutionFile.FullName --configuration Debug --no-restore --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    Write-Host "[HATA] dotnet build basarisiz oldu!" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Proje basariyla derlendi" -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  [OK] KURULUM TAMAMLANDI!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Projeyi calistirmak icin:" -ForegroundColor Yellow
Write-Host "   dotnet run --project App.Presentation" -ForegroundColor White
Write-Host ""
Write-Host "Docker ile calistirmak icin:" -ForegroundColor Yellow
Write-Host "   docker-compose up -d --build" -ForegroundColor White
Write-Host ""
