@echo off
setlocal

REM -------------------------------
REM 配置区
REM -------------------------------
set "REPO=czjnoe/AvaloniaSample"
set "RELEASES_DIR=%~dp0..\src\AvaloniaSample\releases"
set "GITHUB_TOKEN=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"

REM -------------------------------
REM 参数检查
REM -------------------------------
if "%~1"=="" (
    echo Usage: %0 [release-tag]
    echo Example: %0 v1.1.0
    exit /b 1
)
set "RELEASE_TAG=%~1"

REM -------------------------------
REM 检查 gh 命令
REM -------------------------------
where gh >nul 2>&1
if errorlevel 1 (
    echo GitHub CLI not found. Install from https://cli.github.com/
    exit /b 1
)

REM -------------------------------
REM 登录 GitHub（非交互式）
REM -------------------------------
echo %GITHUB_TOKEN% | gh auth login --with-token

REM -------------------------------
REM 创建 Release（如果不存在）
REM -------------------------------
gh release view "%RELEASE_TAG%" --repo "%REPO%" >nul 2>&1
if errorlevel 1 (
    echo Creating release %RELEASE_TAG%...
    gh release create "%RELEASE_TAG%" --repo "%REPO%" --title "%RELEASE_TAG%" --notes "Auto upload Velopack release"
) else (
    echo Release %RELEASE_TAG% already exists.
)

REM -------------------------------
REM 遍历 Releases 文件夹上传
REM -------------------------------
for /R "%RELEASES_DIR%" %%F in (*) do (
    REM 获取相对路径：直接去掉根目录部分
    set "FULLPATH=%%F"
    set "RELPATH=%%F"
    REM 去掉 %RELEASES_DIR% 前缀，使用 call + setlocal 来安全处理
    call :GetRelPath "%%F" "%RELEASES_DIR%" RELPATH
    echo Uploading !RELPATH! ...
    gh release upload "%RELEASE_TAG%" "%%F" --repo "%REPO%" --clobber --name "!RELPATH!"
)

echo ==========================================
echo Upload finished!
pause
exit /b

REM -------------------------------
REM 子程序：获取相对路径
REM -------------------------------
:GetRelPath
REM %1 = 全路径
REM %2 = 根目录
REM %3 = 输出变量名
setlocal enabledelayedexpansion
set "FULL=%~1"
set "BASE=%~2"

REM 确保 BASE 末尾有反斜杠
if not "!BASE:~-1!"=="\" set "BASE=!BASE!\"

set "REL=!FULL:%BASE%=!"
endlocal & set "%3=%REL%"
exit /b