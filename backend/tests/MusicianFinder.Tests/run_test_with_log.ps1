# run_test_detailed.ps1
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$logFile = "test_output_$timestamp.txt"

Write-Host "Запуск теста EndToEndUserScenarioTests с подробным логированием..." -ForegroundColor Cyan
Write-Host "Лог сохраняется в: $logFile" -ForegroundColor Yellow

# Запуск теста с выводом в консоль и в файл
dotnet test --logger "console;verbosity=detailed" --filter "FullyQualifiedName~EndToEndUserScenarioTests" *>&1 | Tee-Object -FilePath $logFile

Write-Host "`nТест завершён. Лог сохранён в $logFile" -ForegroundColor Green
Write-Host "Нажмите любую клавишу, чтобы закрыть окно..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")