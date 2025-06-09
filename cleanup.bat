@echo off
setlocal

REM Delete bin, obj, TestResults, and .vs folders recursively
for /d /r %%d in (bin,obj,TestResults,.vs) do (
    if exist "%%d" (
        echo Deleting folder: %%d
        rmdir /s /q "%%d"
    )
)

echo Cleanup complete.
endlocal
