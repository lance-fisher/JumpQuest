@echo off
title Jamison Gaming's Jump Quest!
cd /d "%~dp0"
set PORT=8075

:: Check if port is already in use
netstat -ano | findstr :%PORT% >nul 2>&1
if %errorlevel%==0 (
    echo Server already running on port %PORT%
) else (
    echo Starting Jamison Gaming's Jump Quest! server...
    start /b python -m http.server %PORT% --bind 127.0.0.1
    timeout /t 1 /nobreak >nul
)

:: Open in Edge (app mode for clean, full-screen look)
start msedge --app=http://localhost:%PORT% --window-size=1280,800

echo.
echo Jamison Gaming's Jump Quest! is running at http://localhost:%PORT%
echo Close this window to stop the server.
echo.
pause
