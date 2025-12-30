
## AvaloniaSample 跨平台桌面应用程序示例

bat脚本打包
build.bat ：项目publish
velopack-pack.bat ：velopack打包
build-and-pack.bat ：项目publish+velopack打包


## 使用Velopack实现更新服务，一款强大的跨平台应用程序安装和自动更新框架
Velopack 文档 https://docs.velopack.io/getting-started/csharp

### 1、安装 Velopack CLI
dotnet tool install -g vpk

### 2、查看 Velopack 版本
dotnet tool list -g | findstr vpk

### 3、查看 Velopack 帮助
vpk pack --help

### 4、Velopack打包成安装程序
vpk pack --packId AvaloniaSample --packVersion 1.0.0 --runtime win-x64 --channel win-x64 --packDir ./bin/Release/net8.0/win-x64/publish --mainExe AvaloniaSample.exe --outputDir ./releases

### 5、使用 build.bat 脚本一键Velopack打包
运行时修改可以修改APP_VERSION、APP_NAME、APP_TITLE、APP_AUTHORS，打包参数APP_VERSION改成相应的版本

