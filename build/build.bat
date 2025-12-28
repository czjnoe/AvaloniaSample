@echo off
chcp 65001 > nul
setlocal EnableDelayedExpansion

:: ========================================
:: 配置区域 - 根据你的项目修改
:: ========================================
set APP_NAME=AvaloniaSample
set APP_VERSION=1.0.0
set APP_TITLE=Avalonia Sample
set APP_AUTHORS=CZJ

:: ⭐ 项目路径配置（相对或绝对路径）
:: 示例1：相对路径
set PROJECT_DIR=..\src\AvaloniaSample

:: 示例2：绝对路径
:: set PROJECT_DIR=E:\code\avalonia-sample\AvaloniaSample

:: 示例3：如果脚本在 scripts 文件夹，项目在上一级
:: set PROJECT_DIR=..

echo.
echo ========================================
echo   跨平台打包脚本
echo ========================================
echo.
echo 项目路径: %PROJECT_DIR%
echo 应用名称: %APP_NAME%
echo 版本号: %APP_VERSION%
echo.
echo ========================================
echo.

:: 检查项目路径是否存在
if not exist "%PROJECT_DIR%" (
    echo [错误] 项目路径不存在: %PROJECT_DIR%
    pause
    exit /b 1
)

:: 进入项目目录
cd /d "%PROJECT_DIR%"
if errorlevel 1 (
    echo [错误] 无法进入项目目录
    pause
    exit /b 1
)

echo [信息] 当前工作目录: %CD%
echo.

:: 检查 .csproj 文件是否存在
if not exist "%APP_NAME%.csproj" (
    echo [错误] 找不到 %APP_NAME%.csproj 文件
    echo [提示] 请确认以下内容：
    echo   1. PROJECT_DIR 路径是否正确
    echo   2. APP_NAME 是否与 .csproj 文件名一致
    dir /b *.csproj 2>nul
    pause
    exit /b 1
)

echo [信息] 找到项目文件: %APP_NAME%.csproj
echo.

:: ========================================
:: 步骤1：清理旧文件
:: ========================================
echo [1/6] 清理旧文件...
if exist "bin\Release" rd /s /q "bin\Release"
if exist "obj\Release" rd /s /q "obj\Release"
if exist "releases" rd /s /q "releases"
echo ✓ 清理完成
echo.

:: ========================================
:: 步骤2：恢复依赖
:: ========================================
echo [2/6] 恢复 NuGet 依赖...
dotnet restore
if errorlevel 1 (
    echo ✗ 依赖恢复失败
    pause
    exit /b 1
)
echo ✓ 依赖恢复完成
echo.

:: ========================================
:: 步骤3：编译和发布所有平台
:: ========================================
echo [3/6] 编译和发布所有平台...
echo.

:: Windows x64
echo   - 发布 Windows x64...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false
if errorlevel 1 (
    echo   ✗ Windows x64 发布失败
    pause
    exit /b 1
)
echo   ✓ Windows x64 发布完成
echo.

:: Windows x86
echo   - 发布 Windows x86...
dotnet publish -c Release -r win-x86 --self-contained true -p:PublishSingleFile=false
if errorlevel 1 (
    echo   ✗ Windows x86 发布失败
    pause
    exit /b 1
)
echo   ✓ Windows x86 发布完成
echo.

echo ✓ 所有平台发布完成
echo.

:: ========================================
:: 步骤4：验证可执行文件
:: ========================================
echo [4/6] 验证可执行文件...
echo.

set "ALL_EXIST=1"

if exist "bin\Release\net8.0\win-x64\publish\%APP_NAME%.exe" (
    echo   ✓ Windows x64: %APP_NAME%.exe
) else (
    echo   ✗ Windows x64: %APP_NAME%.exe 不存在
    set "ALL_EXIST=0"
)

if exist "bin\Release\net8.0\win-x86\publish\%APP_NAME%.exe" (
    echo   ✓ Windows x86: %APP_NAME%.exe
) else (
    echo   ✗ Windows x86: %APP_NAME%.exe 不存在
    set "ALL_EXIST=0"
)

echo.

if "%ALL_EXIST%"=="0" (
    echo [错误] 部分可执行文件不存在！
    pause
    exit /b 1
)

echo ✓ 所有可执行文件验证通过
echo.

:: ========================================
:: 步骤5：使用 Velopack 打包
:: ========================================
echo [5/6] 使用 Velopack 打包...
echo.

mkdir "releases\windows-x64" 2>nul
mkdir "releases\windows-x86" 2>nul

:: 打包 Windows x64
echo   - 打包 Windows x64...
vpk pack ^
  --packId %APP_NAME% ^
  --packVersion %APP_VERSION% ^
  --packDir "bin\Release\net8.0\win-x64\publish" ^
  --mainExe "%APP_NAME%.exe" ^
  --packTitle "%APP_TITLE%" ^
  --packAuthors "%APP_AUTHORS%" ^
  --outputDir "releases\windows-x64"

if errorlevel 1 (
    echo   ✗ Windows x64 打包失败
) else (
    echo   ✓ Windows x64 打包完成
)
echo.

:: 打包 Windows x86
echo   - 打包 Windows x86...
vpk pack ^
  --packId %APP_NAME% ^
  --packVersion %APP_VERSION% ^
  --packDir "bin\Release\net8.0\win-x86\publish" ^
  --mainExe "%APP_NAME%.exe" ^
  --packTitle "%APP_TITLE%" ^
  --packAuthors "%APP_AUTHORS%" ^
  --outputDir "releases\windows-x86"

if errorlevel 1 (
    echo   ✗ Windows x86 打包失败
) else (
    echo   ✓ Windows x86 打包完成
)
echo.

:: ========================================
:: 步骤6：生成打包报告
:: ========================================
echo [6/6] 生成打包报告...
echo.

echo ======================================== > "releases\BUILD_REPORT.txt"
echo   构建报告 >> "releases\BUILD_REPORT.txt"
echo ======================================== >> "releases\BUILD_REPORT.txt"
echo. >> "releases\BUILD_REPORT.txt"
echo 项目路径: %CD% >> "releases\BUILD_REPORT.txt"
echo 应用名称: %APP_NAME% >> "releases\BUILD_REPORT.txt"
echo 版本号: %APP_VERSION% >> "releases\BUILD_REPORT.txt"
echo 构建时间: %date% %time% >> "releases\BUILD_REPORT.txt"
echo. >> "releases\BUILD_REPORT.txt"
echo 打包的平台: >> "releases\BUILD_REPORT.txt"
echo   - Windows x64 >> "releases\BUILD_REPORT.txt"
echo   - Windows x86 >> "releases\BUILD_REPORT.txt"
echo. >> "releases\BUILD_REPORT.txt"

echo.
echo ========================================
echo   打包完成！
echo ========================================
echo.
echo 安装程序位置：
echo.
echo   Windows x64: releases\windows-x64\%APP_NAME%-Setup.exe
echo   Windows x86: releases\windows-x86\%APP_NAME%-Setup.exe
echo.
echo 完整报告: releases\BUILD_REPORT.txt
echo 项目路径: %CD%
echo.

pause