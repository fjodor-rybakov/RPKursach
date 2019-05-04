@echo off
set work_dir=%cd%
start redis-server
cd src\Backend
start "Backend" dotnet Backend.dll
cd %work_dir%
cd src\Components\Auth
start "Auth" dotnet Auth.dll