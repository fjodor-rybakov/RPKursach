@echo off
setlocal enableextensions enabledelayedexpansion
set version=%~1
cd ..
cd ..
set work_dir=%cd%
set scripts=%cd%/scripts/win

set path_build=%work_dir%\build
set path_backend=%work_dir%\src\Backend
set path_showcase=%work_dir%\src\Showcase
set path_auth=%work_dir%\src\Components\Auth

if not defined version (
	cd %scripts%
	echo Version are required.
	exit /B 1
)

if not exist %path_build% mkdir build
cd build

if exist %version% (
	cd %scripts%
	echo This version already exists.
	exit /B 1
)

mkdir %version%
cd %version%
mkdir files\products
mkdir src
cd src
mkdir Components

cd %path_backend%
dotnet publish --output %path_build%\%version%\src\Backend
cd %path_showcase%
dotnet publish --output %path_build%\%version%\src\Showcase
cd %path_auth%
dotnet publish --output %path_build%\%version%\src\Components\Auth

xcopy "%scripts%\run.bat" "%path_build%\%version%"
xcopy "%scripts%\stop.bat" "%path_build%\%version%"

cd %scripts%