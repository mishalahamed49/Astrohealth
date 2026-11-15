# AstroHealth - Run Both Server and Client
# Double-click this file or run in PowerShell to start the full app

Write-Host "Starting AstroHealth Server..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project '$PSScriptRoot\AstroHealth.Server\AstroHealth.Server.csproj' --launch-profile https"

Write-Host "Waiting for Server to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host "Starting AstroHealth Client..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project '$PSScriptRoot\AstroHealth.Client\AstroHealth.Client.csproj' --launch-profile https"

Write-Host "Opening browser..." -ForegroundColor Green
Start-Sleep -Seconds 4
Start-Process "https://localhost:7060"

Write-Host "AstroHealth is running!" -ForegroundColor Green
Write-Host "Server: https://localhost:7189" -ForegroundColor White
Write-Host "Client: https://localhost:7060" -ForegroundColor White
