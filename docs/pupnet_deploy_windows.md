# PupNet-Deploy Windows 打包与发布完整指南

本文档提供使用 `PupNet-Deploy` 将 C# (如 .NET 控制台或 Avalonia 桌面应用) 打包成原生 Windows 安装程序 (`.exe`) 的完整、零坑位操作指南。

---

## 1. 环境准备与依赖安装

在 Windows 下打包 Setup 安装包，底层依赖于 **.NET SDK** 和 **Inno Setup**。

### 1.1 安装 .NET SDK
确保系统已安装 .NET SDK (推荐 .NET 6.0 / 8.0 或以上版本)。可在命令行中运行验证：
```bash
dotnet --version
```

### 1.2 安装 Inno Setup 并配置环境变量（关键）
PupNet 需要调用 Inno Setup 的命令行编译器 `iscc`。
1. 前往 [jrsoftware.org](https://jrsoftware.org/isinfo.php) 下载并安装 Inno Setup 6。
2. 将其安装路径（默认通常为 `C:\Program Files (x86)\Inno Setup 6`）添加到系统的 **环境变量 Path** 中。
3. **验证：** 打开一个**全新**的终端窗口，输入 `iscc`，如果输出版本信息说明配置成功。

### 1.3 全局安装 PupNet-Deploy
在终端运行以下命令将 PupNet 安装为全局 dotnet 工具：
```bash
dotnet tool install -g KuiperZone.PupNet
```

---

## 2. 项目配置文件：`app.pupnet.conf`

在项目根目录下（与 `.csproj` 或 `.sln` 同级）创建一个名为 **`app.pupnet.conf`** 的配置文件。

> **⚠️ 语法特别提醒：** 在 PupNet 配置文件中，**注释必须独立占一行（以 `#` 开头）**，切勿写在参数值的同一行末尾，否则会引发解析错误。

将以下带有详细注释的完整标准配置模板复制并保存到 `app.pupnet.conf` 中：

```ini
# ==========================================
# 1. 核心必填信息 (决定软件名称、入口及基础属性)
# ==========================================

# [核心] 编译生成的主程序可执行文件名称（严格不带 .exe 后缀）。
# PupNet 在 dotnet publish 后会去输出目录寻找对应的 .exe，名字对不上会报 File not found。
AppBaseName = AvaloniaSample

# 软件的友好显示名称。
# 用于生成 Windows 桌面快捷方式名称、开始菜单名称、以及控制面板“卸载程序”列表里的显示名称。
AppFriendlyName = MyAvaloniaSample

# 软件的全局唯一标识符（采用反向域名格式）。
# 在 Windows 下会作为 InnoSetup 的内部 AppId（用于识别是否为同一个软件的覆盖升级安装）；Linux 下作为包名标识。
AppId = com.mycompany.mydemoapp

# 软件的发布版本号。
# 会写入安装包属性中，并且可以通过内置宏 ${APP_VERSION} 自动传递给下方的 dotnet publish 命令。
AppVersionRelease = 1.0.0

# 软件的一句话简短描述。
# 在 Windows 下会作为安装程序文件属性里的“文件说明”；在 Linux 下作为包管理器的软件摘要。
AppShortSummary = 这是一个Windows桌面应用程序

# 软件的开源许可证或商业协议标识符（遵循 SPDX 国际标准格式）。
# 开源常用 MIT、Apache-2.0、GPL-3.0；如果是闭源商业软件，可填 Proprietary。
AppLicenseId = MIT

# 软件的主分类（遵循 Freedesktop 菜单分类标准）。
# 主要用于 Linux 桌面环境归类（如 Utility 工具, Development 开发, Office 办公, Game 游戏等），Windows 下仅做校验。
PrimeCategory = Utility


# ==========================================
# 2. 发行商必填信息 (显示在安装包版权与卸载列表中)
# ==========================================

# 发行商或开发者名称。
# 在 Windows 安装向导、软件属性“版权/公司”栏、以及控制面板卸载列表的“发布者”一栏中显示。
PublisherName = MyCompany

# 发行商的反向域名标识。
# 主要用于 Linux 元数据规范校验，通常取 AppId 的前两段即可。
PublisherId = com.mycompany

# 发行商的联系或技术支持邮箱。
# 会写入软件安装包的技术支持联系信息中。
PublisherEmail = admin@mycompany.com


# ==========================================
# 3. .NET 编译参数 (控制 dotnet publish 的具体行为)
# ==========================================

# 指定要编译的 .NET 项目路径（相对于本 .pupnet.conf 文件所在的目录）。
# "." 代表当前目录。如果是 Avalonia 多项目结构，建议精确指向桌面端项目，例如：AvaloniaSample.Desktop/AvaloniaSample.Desktop.csproj
DotnetProjectPath = .

# 传递给 `dotnet publish` 的附加命令行参数：
#    -p:Version=${APP_VERSION} : 将上方配置的 AppVersionRelease 版本号同步注入到编译出的 exe 文件属性中
#    --self-contained true     : 独立部署模式（将 .NET 运行时一起打包，用户电脑无需安装 .NET 环境即可运行）
#    -p:DebugType=None         : 不生成调试信息
#    -p:DebugSymbols=false     : 不生成 .pdb 调试符号文件，减小安装包体积
DotnetPublishArgs = -p:Version=${APP_VERSION} --self-contained true -p:DebugType=None -p:DebugSymbols=false


# ==========================================
# 4. Linux Flatpak 必填项 (重要：打包 Windows 也不可删除！)
# 作用说明：PupNet 启动时会强制检查所有跨平台必填字段是否为空。
# 在 Windows 下打包时，下面这 3 项仅用于应付启动校验，实际构建过程中完全不会被调用。
# ==========================================

# Flatpak 运行时的基础平台名称
FlatpakPlatformRuntime = org.freedesktop.Platform

# Flatpak 编译构建所使用的 SDK 名称
FlatpakPlatformSdk = org.freedesktop.Sdk

# Flatpak 基础平台与 SDK 的版本号
FlatpakPlatformVersion = 25.08


# ==========================================
# 5. Windows Setup (InnoSetup) 专属控制项
# ==========================================

# 允许安装此软件的最低 Windows 版本号。
# 填 10 代表要求 Windows 10 及以上系统才允许运行安装程序。
SetupMinWindowsVersion = 10

# 是否强制要求管理员权限（UAC 弹窗）才能安装：
#    false = 普通用户模式安装（默认安装到用户目录 AppData\Local\Programs 下，无需管理员密码，最省心）
#    true  = 管理员模式安装（默认安装到 C:\Program Files 下，安装时会弹出管理员权限确认框）
SetupAdminInstall = false

# 是否在最终生成的 Setup.exe 安装包文件名中包含版本号：
#    true  = 生成类似 AvaloniaSample-1.0.0-Setup.exe 的文件名
#    false = 生成类似 AvaloniaSample-Setup.exe 的文件名
SetupVersionOutput = true

# 是否隐藏桌面快捷方式和开始菜单入口：
#    false = 正常GUI桌面软件（安装后在开始菜单和桌面生成快捷方式）
#    true  = 不生成快捷方式（通常用于后台服务或纯命令行工具）
DesktopNoDisplay = false

# 软件运行时是否需要附带弹出黑色的命令行终端窗口：
#    false = 纯图形界面程序（Avalonia/WPF/WinForms 必须设为 false，否则启动软件背后会跟着一个黑框）
#    true  = 控制台命令行程序（Console App 需要设为 true）
DesktopTerminal = false
```

---

## 3. 执行一键打包

在配置文件所在的目录下打开终端，运行显式指定配置文件的打包命令：

```bash
pupnet app.pupnet.conf --runtime win-x64 --kind setup
```

**执行流程说明：**
1. PupNet 读取配置并校验所有字段。
2. 自动在后台调用 `dotnet publish` 进行独立发布编译。
3. 自动生成 Inno Setup 脚本并调用 `iscc` 压制出最终的安装包。
4. 打包成功后，你可以在项目输出目录下找到生成的 Setup 安装程序。

---

## 4. 常见问题排查指南

1. **报错 `Mandatory value required for Configuration ...`**
   * 说明配置文件里漏掉了某些跨平台必填字段（如 `PublisherName` 或 `FlatpakPlatformRuntime` 等），直接复制上方完整模板即可解决。

2. **报错 `File not found ... AppDir\Publish\xxx.exe`**
   * 说明 `DotnetProjectPath` 指向不对或者 `AppBaseName` 与实际生成的 `.exe` 文件名不一致。请检查实际编译出的主程序名称，并同步修改配置文件中的 `AppBaseName`。

3. **安装向导语言如何支持中文？**
   * 可将 Inno Setup 简体中文语言包（`ChineseSimplified.isl`）放入 Inno Setup 的 `Languages` 目录中，并在编译生成的临时 `.iss` 脚本中引入该语言包，即可让安装向导变为纯中文界面。