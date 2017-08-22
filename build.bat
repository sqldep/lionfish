set BUILDDIR=build_%TIME::=-%
mkdir "%BUILDDIR%"

rmdir /S/Q SQLDep\bin\Release
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" SQLDepCmd/SQLDepCmd.csproj  /build Release
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" SQLDep/SQLDep.csproj  /build Release

xcopy SQLDepCmd\bin\Release\SQLDepCmd.exe SQLDep\bin\Release
xcopy /s sql SQLDep\bin\Release\sql\
xcopy export.bat SQLDep\bin\Release
xcopy README.md SQLDep\bin\Release
xcopy packages\Terradata\Teradata.Net.Security.Tdgss.dll SQLDep\bin\Release
del /s SQLDep\bin\Release\*.pdb

xcopy /s SQLDep\bin\Release %BUILDDIR%\SQLdep\
cd %BUILDDIR%
zip -r SQLdep.zip SQLdep\
