@echo off
start redis-server
start "Backend" dotnet src\Backend\Backend.dll
start "Auth" dotnet src\Components\Auth\Auth.dll