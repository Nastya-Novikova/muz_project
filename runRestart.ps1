Write-Host "Сборка и запуск" -ForegroundColor Cyan

Set-Location -Path $PSScriptRoot

cd backend

rm -r Migrations

dotnet ef migrations add InitialCreate

dotnet ef migrations script -o ../init.sql

cd ..

docker-compose down

docker-compose down -v

Write-Host "🛑 Остановлено." -ForegroundColor Yellow