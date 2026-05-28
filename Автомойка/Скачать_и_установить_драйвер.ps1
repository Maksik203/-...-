# Запуск: правый щелчок -> "Выполнить с помощью PowerShell"
# Или: Win+X -> Терминал (администратор) -> cd к папке -> .\Скачать_и_установить_драйвер.ps1

$ErrorActionPreference = "Stop"
$url = "https://download.microsoft.com/download/3/5/C/35C84C36-661A-44E6-9324-8786B8DBE231/AccessDatabaseEngine_X64.exe"
$installer = Join-Path $env:TEMP "AccessDatabaseEngine_X64.exe"

Write-Host ""
Write-Host "=== Установка драйвера Access для HotelIS ===" -ForegroundColor Cyan
Write-Host ""

if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "Нужны права администратора. Перезапуск..." -ForegroundColor Yellow
    Start-Process powershell.exe "-ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs
    exit
}

Write-Host "1/2 Скачивание (~80 МБ)..." -ForegroundColor Green
Invoke-WebRequest -Uri $url -OutFile $installer -UseBasicParsing

Write-Host "2/2 Установка (1-3 минуты)..." -ForegroundColor Green
$proc = Start-Process -FilePath $installer -ArgumentList "/quiet" -Wait -PassThru

if ($proc.ExitCode -ne 0) {
    Write-Host "Тихая установка не удалась, пробуем /passive..." -ForegroundColor Yellow
    Start-Process -FilePath $installer -ArgumentList "/passive" -Wait
}

Write-Host ""
Write-Host "Готово! Запустите HotelIS (F5 в Visual Studio)." -ForegroundColor Green
Write-Host ""
Read-Host "Нажмите Enter для выхода"
