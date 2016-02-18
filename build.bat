rmdir SQLDep/bin/Release
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" /build SQLDepCmd/SQLDepCmd.csproj
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" /build SQLDep/SQLDep.csproj
cp -r sql SQLDep/bin/Release
cp export.bat SQLDep/bin/Release
cp README.md SQLDep/bin/Release
cp packages/Terradata/Teradata.net.dll SQLDep/bin/Release
cp -r SQLDep/bin/Release SQLDep/bin/SQLdep
zip sqldep.zip SQLDep/bin/SQLdep/
