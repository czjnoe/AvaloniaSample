
## Velopack 一款强大的跨平台应用程序安装和自动更新框架

### 1、安装Velopack打包工具
dotnet tool install -g vpk

### 2、Velopack打包成安装程序
vpk pack --packId AvaloniaSample --packVersion 1.0.0 --packDir ./bin/Release/net8.0/publish/win-x64 --mainExe AvaloniaSample.exe --outputDir ./releases

### 3、使用 build.bat 脚本一键打包
运行时修改可以修改APP_VERSION、APP_NAME、APP_TITLE、APP_AUTHORS