using Avalonia;
using AvaloniaSample.Helper;
using ReactiveUI.Avalonia;
using System;

namespace AvaloniaSample
{
    /// <summary>
    /// 使用文档：https://docs.avaloniaui.net/zh-Hans/docs/get-started/install
    /// </summary>
    internal sealed class Program
    {
        public static AppSingleInstanceHelper? Instance;

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            Instance = new AppSingleInstanceHelper("MyCompany.MyAvaloniaApp");
            if (!Instance.IsFirstInstance)
            {
                Instance.SendArgumentsToFirstInstanceAsync(args)
                         .GetAwaiter().GetResult();
                return;
            }

            // 配置 Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()  // 设置最低日志级别
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)  // 覆盖特定命名空间
                .Enrich.FromLogContext()  // 添加上下文信息
                .WriteTo.Console()  // 输出到控制台
                .WriteTo.File(
                    path: "logs/app.log",
                    rollingInterval: RollingInterval.Day,  // 按天滚动
                    fileSizeLimitBytes: 10485760,  // 10MB
                    retainedFileCountLimit: 7,  // 保留7天
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.Debug()  // 输出到调试窗口
                .CreateLogger();

            try
            {
                Log.Information("应用程序启动中...");
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "应用程序意外终止");
                throw;
            }
            finally
            {
                Instance.Dispose();
                Log.CloseAndFlush();  // 确保日志写入完成
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()   // Avalonia 自身日志
                .UseReactiveUI();
    }
}
