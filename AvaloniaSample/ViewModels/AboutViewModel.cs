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
    public class AboutViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> OpenSourceLink { get; set; } = ReactiveCommand.Create(OpenSourceLinkClick);

        public string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";

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

        private static void OpenSourceLinkClick()
        {
            Process.Start(new ProcessStartInfo("https://gitee.com/czjnoe/avalonia-sample.git") { UseShellExecute = true });
        }
    }
}
