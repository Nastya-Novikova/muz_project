$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$logFile = "test_log_$timestamp.txt"

Write-Host "Running test and logging to $logFile ..."

# Выполняем команду, захватывая все потоки (stdout, stderr) и пишем в файл
& dotnet test --filter "FullyQualifiedName~EndToEndUserScenarioTests" *>&1 | Tee-Object -FilePath $logFile

Write-Host "Test finished. Opening log..."
Start-Process notepad.exe $logFile