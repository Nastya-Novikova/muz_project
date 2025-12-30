
Set-Location -Path $PSScriptRoot

docker-compose down

docker-compose down -v

Write-Host "Сборка и запуск" -ForegroundColor Cyan

docker-compose up --build

Write-Host "🛑 Остановлено." -ForegroundColor Yellow