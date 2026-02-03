@echo off
echo Publishing .NET Project...

REM Project ka path
set PROJECT_PATH=D:\TestAjax\TestAjax.csproj

REM Output publish folder
set PUBLISH_PATH=D:\TestAjax\PublishMyApp

dotnet publish "%PROJECT_PATH%" -c Release -o "%PUBLISH_PATH%"

echo Publish complete!
pause
