@echo off
set SOLUTION_NAME=WindowResizer
set SOLUTION_FILE=%SOLUTION_NAME%\%SOLUTION_NAME%.csproj
set BUILD_CONFIG=Release
set MAIN_PROJECT_OUTDIR=%SOLUTION_NAME%\bin\%BUILD_CONFIG%\net8.0-windows7.0
set ARTIFACTS_BASEDIR=Artifacts
set ARTIFACTS_SUBDIR=%SOLUTION_NAME%-winx64
set ARTIFACTS_NAME=%ARTIFACTS_SUBDIR%.zip

dotnet publish --nologo ^
-c %BUILD_CONFIG% ^
-o %ARTIFACTS_BASEDIR%\%ARTIFACTS_SUBDIR% ^
-p:PublishReadyToRun=true ^
-p:PublishSingleFile=true ^
-p:PublishReadyToRunShowWarnings=true ^
--self-contained true ^
-r win-x64 ^
%SOLUTION_FILE% || set ERRORLEVEL=0

del %ARTIFACTS_BASEDIR%\%ARTIFACTS_SUBDIR%\*.pdb

set CMD7Z_FULLPATH="C:\Program Files\7-Zip\7z.exe"
set CMD7Z=
where 7z.exe 2>NUL >NUL && set CMD7Z=7z.exe
if [%CMD7Z%]==[] (
  if exist %CMD7Z_FULLPATH% (
    set CMD7Z=%CMD7Z_FULLPATH%
  )
)

if exist %ARTIFACTS_NAME% (
  del %ARTIFACTS_NAME%
)

cd %ARTIFACTS_BASEDIR%
if [%CMD7Z%]==[] (
  powershell Compress-Archive -Path %ARTIFACTS_SUBDIR% -DestinationPath ..\%ARTIFACTS_NAME%
) else (
  %CMD7Z% a -mmt=on -mm=Deflate -mfb=258 -mpass=15 -r ..\%ARTIFACTS_NAME% %ARTIFACTS_SUBDIR%
)
cd ..
