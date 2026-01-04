using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaSample.Consts;
using AvaloniaSample.RegionAdapters;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.ViewModels
{
    public class AboutViewModel : ViewModelBase, ITabItemBase
    {
        private readonly IContainerProvider _container;

        public ReactiveCommand<Unit, Unit> OpenSourceLink { get; set; } = ReactiveCommand.Create(OpenSourceLinkClick);

        public string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

        public string Platform { get; } = RuntimeInformation.RuntimeIdentifier ?? "Unknown";

        public string Framework { get; } = RuntimeInformation.FrameworkDescription ?? "Unknown";

        public string BuildDate { get; } = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("yyyy-MM-dd");

        public static string OSString => $"{OS} {Environment.OSVersion.Version}";

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        private static string OS
        {
            get
            {
                if (OperatingSystem.IsWindows())
                    return "Windows";
                if (OperatingSystem.IsMacOS())
                    return "macOS";
                if (OperatingSystem.IsLinux())
                    return "Linux";
                return "Unknown";
            }
        }

        public ReactiveCommand<Unit, Unit> CheckUpdateCommand { get; set; } =
        ReactiveCommand.Create(CheckUpdateClick);

        public string? TitleKey { get; set; } = Resources.Resources.Tab_About_Title;
        public string? MessageKey { get; set; } = Resources.Resources.Tab_About_Description;

        public AboutViewModel()
        {

        }

        private static void OpenSourceLinkClick()
        {
            Process.Start(new ProcessStartInfo(GlobalConst.GitPath) { UseShellExecute = true });
        }

        /// <summary>
        /// 手动检查更新
        /// </summary>
        private static async void CheckUpdateClick()
        {
            await ShowUpdateWindowAsync();
        }

        /// <summary>
        /// 显示更新窗口
        /// </summary>
        private static async Task ShowUpdateWindowAsync()
        {
            try
            {
                // 从容器解析 UpdateViewModel
                var updateViewModel = ContainerLocator.Container.Resolve<UpdateViewModel>();

                // 创建更新窗口
                var updateWindow = ContainerLocator.Container.Resolve<UpdateWindow>();
                updateWindow.DataContext = updateViewModel;

                // 显示窗口
                await updateWindow.ShowDialog(App.Instance.MainWindow as Window);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"显示更新窗口失败: {ex.Message}");
            }
        }
    }
}
