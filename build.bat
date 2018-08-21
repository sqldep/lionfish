chcp 65001
set devenvPath="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe"

%devenvPath% SQLDepCmd/SQLDepCmd.csproj  /build Release
%devenvPath% SQLDep/SQLDep.csproj  /build Release

rmdir /S/Q Release
mkdir Release
mkdir Release\sql
xcopy /Y/Q SQLDep\bin\x86\Release\* Release
xcopy /Y/Q SQLDepCmd\bin\x86\Release\* Release
xcopy /S/Q sql Release\sql
del /s Release\*.pdb


rem To sign .exe files, uncomment next 3 lines, change path to signtool and fill in *values*

rem set signtoolPath="C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool\signtool.exe"
rem %signtoolPath% sign /f *certificatePath* /p *CertificatePassword* /t http://timestamp.comodoca.com Release\SQLdepCmd.exe
rem %signtoolPath% sign /f *certificatePath* /p *CertificatePassword* /t http://timestamp.comodoca.com Release\SQLdep.exe

pause