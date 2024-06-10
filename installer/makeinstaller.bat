@echo off
chcp 65001
REM set version no in appsettings.json before build
REM signing certificate password qaws123.

rmdir /s /q pack
mkdir pack

set VERSNO=1.0.0
REM cd ../spa
REM ng build --configuration "production"
cd ..
dotnet publish EMVLogger.csproj -c Release -r win-x86 --output ./installer/pack -p:PublishSingleFile=true
cd ./installer

mkdir .\pack\wwwroot
xcopy /E /Y "..\spa\dist\spa" ".\pack\wwwroot"

del /f .\pack\appsettings.Development.json

makensis /DVERSNO="%VERSNO%" EMVLogger.nsi

PAUSE
