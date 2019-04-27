@echo off
setlocal enableextensions enabledelayedexpansion
set version=%~1
cd ..
set work_dir=%cd%

set path_build=%work_dir%\build
set path_backend=%work_dir%\src\Backend

if defined version (
	if not exist %path_build% (
		mkdir build
	)
	cd build
	if not exist %version% (
		mkdir %version%
		cd %version%
	) else (
		cd..
		echo This version already exists.
	)
) else (
	echo Version are required.
)

cd scripts