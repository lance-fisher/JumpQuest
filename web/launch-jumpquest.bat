@echo off
title Jamison Gaming's Jump Quest!
cd /d "%~dp0"

:: Check if port 8095 is already in use
netstat -ano | findstr :8095 >nul 2>&1
if %errorlevel%==0 (
    echo Server already running on port 8095
) else (
    echo Starting Jamison Gaming's Jump Quest! server...
    start /b python -m http.server 8095 --bind 127.0.0.1
    timeout /t 1 /nobreak >nul
)

:: Open in Edge (app mode for clean, full-screen look)
start msedge --app=http://localhost:8095 --window-size=1280,800

echo.
echo Jamison Gaming's Jump Quest! is running at http://localhost:8095
echo Close this window to stop the server.
echo.
pause
