Write-Host "Сборка и запуск" -ForegroundColor Cyan

Set-Location -Path $PSScriptRoot

docker-compose -f .\docker-compose.backend.yml up --build

Write-Host "🛑 Остановлено." -ForegroundColor Yellow