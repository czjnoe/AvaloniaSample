
## Velopack 一款强大的跨平台应用程序安装和自动更新框架
Velopack 文档 https://docs.velopack.io/getting-started/csharp

### 1、安装 Velopack CLI
dotnet tool install -g vpk

### 2、查看 Velopack 版本
vpk --version 

### 3、Velopack打包成安装程序
vpk pack --packId AvaloniaSample --packVersion 1.0.0 --packDir ./bin/Release/net8.0/publish/win-x64 --mainExe AvaloniaSample.exe --outputDir ./releases

### 4、使用 build.bat 脚本一键Velopack打包
运行时修改可以修改APP_VERSION、APP_NAME、APP_TITLE、APP_AUTHORS，打包参数APP_VERSION改成相应的版本

### 5、Velopack 包手动发布到Github Releases