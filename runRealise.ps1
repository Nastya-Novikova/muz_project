Write-Host "Сборка и запуск" -ForegroundColor Cyan

Set-Location -Path $PSScriptRoot

try {
    docker-compose up --build
}
catch {
    Write-Host "❌ Произошла ошибка:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
}
finally {
    Write-Host "🛑 Остановлено." -ForegroundColor Yellow
    Read-Host "Нажмите Enter для закрытия окна"