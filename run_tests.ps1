<#
.SYNOPSIS
Запускает тесты MusicianFinder.Tests и сохраняет вывод в лог-файл.
#>
param(
    [string]$LogFile = "test_log.txt",
    [switch]$NoBuild
)

$scriptRoot = $PSScriptRoot
if (-not $scriptRoot) { $scriptRoot = Get-Location }

$projectPath = Join-Path $scriptRoot "backend\tests\MusicianFinder.Tests\MusicianFinder.Tests.csproj"
if (-not (Test-Path $projectPath)) {
    Write-Error "Не удалось найти тестовый проект по пути: $projectPath"
    exit 1
}

$logFullPath = Join-Path $scriptRoot $LogFile
Write-Host "Запуск тестов. Лог будет сохранён в: $logFullPath"

$testArgs = @("test", $projectPath)
if ($NoBuild) { $testArgs += "--no-build" }

& dotnet $testArgs 2>&1 | Tee-Object -FilePath $logFullPath

$exitCode = $LASTEXITCODE
Write-Host "Тесты завершены с кодом возврата $exitCode. Лог сохранён: $logFullPath"
exit $exitCode