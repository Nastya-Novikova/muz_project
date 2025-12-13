Write-Host "Сборка и запуск" -ForegroundColor Cyan

Set-Location -Path $PSScriptRoot

docker-compose up --build

Write-Host "🛑 Остановлено." -ForegroundColor Yellow