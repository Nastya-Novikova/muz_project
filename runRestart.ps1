Write-Host "Генерация миграций и скрипта init.sql" -ForegroundColor Cyan

Set-Location -Path $PSScriptRoot

Set-Location backend/src/MusicianFinder.Infrastructure

# Удаляем старые миграции, если есть
if (Test-Path "Migrations") {
    Remove-Item -Recurse -Force "Migrations"
    Write-Host "Старые миграции удалены."
}

# Создаём новую миграцию
dotnet ef migrations add InitialCreate `
    --startup-project ../MusicianFinder.API/MusicianFinder.API.csproj `
    --project MusicianFinder.Infrastructure.csproj

# Генерируем SQL-скрипт в корень репозитория (muz_project)
dotnet ef migrations script `
    --startup-project ../MusicianFinder.API/MusicianFinder.API.csproj `
    --project MusicianFinder.Infrastructure.csproj `
    -o ../../../../init.sql

Write-Host "Скрипт init.sql создан в корне репозитория." -ForegroundColor Green

# Возвращаемся в корень
Set-Location -Path $PSScriptRoot

Write-Host "Остановка контейнеров и удаление томов" -ForegroundColor Yellow
docker-compose -f docker-compose.backend.yml down -v

Write-Host "Готово. Теперь можно запустить:" -ForegroundColor Green
Write-Host "docker-compose -f docker-compose.backend.yml up --build" -ForegroundColor White