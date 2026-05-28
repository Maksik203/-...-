@echo off
chcp 65001 >nul
title Установка драйвера Access (ACE) для HotelIS
color 0A

echo.
echo  ============================================================
echo   УСТАНОВКА ДРАЙВЕРА ACCESS (обязательно для HotelIS)
echo  ============================================================
echo.
echo  Скачать файл недостаточно — нужно УСТАНОВИТЬ .exe
echo  Программа HotelIS работает в 64-bit — нужен X64 драйвер.
echo.

set "INSTALLER="
for %%F in (
  "%USERPROFILE%\Downloads\accessdatabaseengine_X64.exe"
  "%USERPROFILE%\Downloads\AccessDatabaseEngine_X64.exe"
  "%USERPROFILE%\Desktop\accessdatabaseengine_X64.exe"
  "%USERPROFILE%\Desktop\Кожевников Влад\accessdatabaseengine_X64.exe"
  "%~dp0accessdatabaseengine_X64.exe"
) do if exist %%~F set "INSTALLER=%%~F"

if not defined INSTALLER (
  echo  [ОШИБКА] Файл accessdatabaseengine_X64.exe не найден.
  echo.
  echo  1. Откройте в браузере:
  echo     https://www.microsoft.com/download/details.aspx?id=54920
  echo  2. Скачайте accessdatabaseengine_X64.exe
  echo  3. Сохраните в папку Загрузки
  echo  4. Запустите этот bat-файл снова
  echo.
  echo.
  echo  Или запустите автоматическую установку:
  echo  правый щелчок по "Скачать_и_установить_драйвер.ps1" -^> Выполнить
  echo.
  start https://www.microsoft.com/download/details.aspx?id=54920
  pause
  exit /b 1
)

echo  Найден установщик:
echo  %INSTALLER%
echo.
echo  Сейчас запустится установка (режим /passive).
echo  Если Windows спросит разрешение — нажмите ДА.
echo.
pause

echo  Установка...
"%INSTALLER%" /passive

if errorlevel 1 (
  echo.
  echo  Обычная установка не удалась. Пробуем от имени администратора...
  powershell -Command "Start-Process -FilePath '%INSTALLER%' -ArgumentList '/passive' -Verb RunAs -Wait"
)

echo.
echo  Готово. Запустите HotelIS снова (F5 в Visual Studio).
echo.
pause
