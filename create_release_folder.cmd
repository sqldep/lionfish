rmdir /S/Q Release
mkdir Release
mkdir Release\sql
xcopy /Y/Q SQLDep\bin\x86\Release\* Release
xcopy /Y/Q SQLDepCmd\bin\x86\Release\* Release
xcopy /S/Q sql Release\sql
