@echo off
setlocal enableextensions enabledelayedexpansion
set version=%~1
cd ..
set work_dir=%cd%
set scripts=%cd%/scripts

set path_build=%work_dir%\build
set path_backend=%work_dir%\src\Backend
set path_auth=%work_dir%\src\Components\Auth

set path_backend_publish=%path_backend%\bin\Debug\netcoreapp2.2\publish
set path_auth_publish=%path_auth%\bin\Debug\netcoreapp2.2\publish

if not defined version (
	echo Version are required.
	exit /B 1
)
if not exist %path_build% mkdir build
cd build
if exist %version% (
	cd..
	echo This version already exists.
	exit /B 1
)
mkdir %version%
cd %version%

mkdir src
cd src

mkdir Components
cd Components

cd %path_backend%
dotnet publish /p:PublishDir=%path_build%\%version%\src\Backend
cd %path_auth%
dotnet publish /p:PublishDir=%path_build%\%version%\src\Components\Auth

cd %path_build%/%version%

xcopy "%scripts%\run.bat" "%path_build%\%version%"
xcopy "%scripts%\stop.bat" "%path_build%\%version%"

cd %scripts%