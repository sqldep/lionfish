REM
REM Copyright (c) 1999, 2004, Oracle. All rights reserved.  
REM
@echo off
set ICINCHOME=..\include
set ICLIBHOME=..\lib\msvc
if (%1) == () goto usage
if (%1) == (cdemo81) goto ocimake
if (%1) == ("cdemo81") goto ocimake
if (%1) == (CDEMO81) goto ocimake
if (%1) == ("CDEMO81") goto ocimake

if (%1) == (occidml) goto occimake
if (%1) == ("occidml") goto occimake
if (%1) == (OCCIDML) goto occimake
if (%1) == ("OCCIDML") goto occimake

cl -I%ICINCHOME% -I. -D_DLL -D_MT %1.c /link /LIBPATH:%ICLIBHOME% oci.lib kernel32.lib msvcrt.lib /nod:libc
goto end

:ocimake
cl -I%ICINCHOME% -I. -D_DLL -D_MT %1.c /link /LIBPATH:%ICLIBHOME% oci.lib msvcrt.lib /nod:libc
goto end

:occimake
cl -GX -DWIN32COMMON -I. -I%ICINCHOME% -I. -D_DLL -D_MT %1.cpp /link /LIBPATH:%ICLIBHOME% oci.lib msvcrt.lib msvcprt.lib oraocci10.lib /nod:libc
goto end

:usage
echo.
echo Usage: make filename [i.e. make cdemo81]
echo.
:end
set ICINCHOME=
set ICLIBHOME=
