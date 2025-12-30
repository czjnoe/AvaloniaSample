@echo off
chcp 65001 > nul
setlocal EnableDelayedExpansion

:: ========================================
:: 配置区域 - 根据你的项目修改
:: ========================================

:: ⭐ 项目路径配置（相对或绝对路径）
:: 示例1：相对路径
set PROJECT_DIR=..\src\AvaloniaSample

:: 示例2：绝对路径
:: set PROJECT_DIR=E:\code\avalonia-sample\AvaloniaSample

:: 示例3：如果脚本在 scripts 文件夹，项目在上一级
:: set PROJECT_DIR=..

:: ⭐ .csproj 文件名（不含路径）
set CSPROJ_FILE=AvaloniaSample.csproj

:: ⭐ 输出目录配置
set OUTPUT_DIR=releases

echo.
echo ========================================
echo   跨平台打包脚本（自动读取配置）
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
if not exist "%CSPROJ_FILE%" (
    echo [错误] 找不到 %CSPROJ_FILE% 文件
    echo [提示] 请确认以下内容：
    echo   1. PROJECT_DIR 路径是否正确
    echo   2. CSPROJ_FILE 文件名是否正确
    echo.
    echo 当前目录中的 .csproj 文件：
    dir /b *.csproj 2>nul
    pause
    exit /b 1
)

echo [信息] 找到项目文件: %CSPROJ_FILE%
echo.

:: ========================================
:: 从 .csproj 文件中读取配置
:: ========================================
echo [0/6] 读取项目配置...
echo.

:: 创建 PowerShell 脚本文件
echo $xml = [xml](Get-Content '%CSPROJ_FILE%') > temp_read.ps1
echo $pg = $xml.Project.PropertyGroup >> temp_read.ps1
echo if ($pg -is [array]) { $pg = $pg[0] } >> temp_read.ps1
echo $assemblyName = if ($pg.AssemblyName) { $pg.AssemblyName } else { '%CSPROJ_FILE%'.Replace('.csproj', '') } >> temp_read.ps1
echo $version = if ($pg.Version) { $pg.Version } elseif ($pg.AssemblyVersion) { $pg.AssemblyVersion } else { '1.0.0' } >> temp_read.ps1
echo $title = if ($pg.Title) { $pg.Title } elseif ($pg.Product) { $pg.Product } else { $assemblyName } >> temp_read.ps1
echo $authors = if ($pg.Authors) { $pg.Authors } elseif ($pg.Company) { $pg.Company } else { 'Author' } >> temp_read.ps1
echo Write-Output "SET APP_NAME=$assemblyName" >> temp_read.ps1
echo Write-Output "SET APP_VERSION=$version" >> temp_read.ps1
echo Write-Output "SET APP_TITLE=$title" >> temp_read.ps1
echo Write-Output "SET APP_AUTHORS=$authors" >> temp_read.ps1

:: 执行 PowerShell 脚本
powershell -ExecutionPolicy Bypass -File temp_read.ps1 > temp_config.bat

if errorlevel 1 (
    echo [错误] 读取 .csproj 文件失败
    pause
    exit /b 1
)

:: 执行生成的配置
call temp_config.bat
del temp_config.bat
del temp_read.ps1

:: 验证读取结果
if "%APP_NAME%"=="" (
    echo [错误] 无法读取 AssemblyName，使用默认值
    for %%F in ("%CSPROJ_FILE%") do set APP_NAME=%%~nF
)

if "%APP_VERSION%"=="" (
    echo [警告] 无法读取 Version，使用默认值
    set APP_VERSION=1.0.0
)

if "%APP_TITLE%"=="" (
    set APP_TITLE=%APP_NAME%
)

if "%APP_AUTHORS%"=="" (
    set APP_AUTHORS=Author
)

echo [配置信息]
echo   应用名称 (AssemblyName): %APP_NAME%
echo   版本号 (Version): %APP_VERSION%
echo   标题 (Title): %APP_TITLE%
echo   作者 (Authors): %APP_AUTHORS%
echo   输出目录: %OUTPUT_DIR%
echo.
echo ✓ 配置读取完成
echo.

:: ========================================
:: 步骤1：清理旧文件
:: ========================================
echo [1/6] 清理旧文件...
if exist "bin\Release" rd /s /q "bin\Release"
if exist "obj\Release" rd /s /q "obj\Release"
if exist "%OUTPUT_DIR%" rd /s /q "%OUTPUT_DIR%"
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

mkdir "%OUTPUT_DIR%\windows-x64" 2>nul
mkdir "%OUTPUT_DIR%\windows-x86" 2>nul

:: 打包 Windows x64
echo   - 打包 Windows x64...
vpk pack ^
  --packId %APP_NAME% ^
  --packVersion %APP_VERSION% ^
  --packDir "bin\Release\net8.0\win-x64\publish" ^
  --mainExe "%APP_NAME%.exe" ^
  --packTitle "%APP_TITLE%" ^
  --packAuthors "%APP_AUTHORS%" ^
  --runtime win-x64 ^
  --channel win-x64 ^
  --outputDir "%OUTPUT_DIR%\windows-x64"

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
  --runtime win-x86 ^
  --channel win-x86 ^
  --outputDir "%OUTPUT_DIR%\windows-x86"

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

echo ======================================== > "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo   构建报告 >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo ======================================== >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo. >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo 项目路径: %CD% >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo 项目文件: %CSPROJ_FILE% >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo 应用名称: %APP_NAME% >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo 版本号: %APP_VERSION% >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo 标题: %APP_TITLE% >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo 作者: %APP_AUTHORS% >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo 输出目录: %OUTPUT_DIR% >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo 构建时间: %date% %time% >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo. >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo 打包的平台: >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo   - Windows x64 >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo   - Windows x86 >> "%OUTPUT_DIR%\BUILD_REPORT.txt"
echo. >> "%OUTPUT_DIR%\BUILD_REPORT.txt"

echo.
echo ========================================
echo   打包完成！
echo ========================================
echo.
echo 安装程序位置：
echo.
echo   Windows x64: %OUTPUT_DIR%\windows-x64\%APP_NAME%-Setup.exe
echo   Windows x86: %OUTPUT_DIR%\windows-x86\%APP_NAME%-Setup.exe
echo.
echo 完整报告: %OUTPUT_DIR%\BUILD_REPORT.txt
echo 项目路径: %CD%
echo.

pause