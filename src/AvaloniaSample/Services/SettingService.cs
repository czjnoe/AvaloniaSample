using Avalonia.Media;
using AvaloniaSample.Consts;
using AvaloniaSample.Helper;
using AvaloniaSample.Interfaces;
using System.Globalization;

namespace AvaloniaSample.Services
{
    public class SettingService : ISettingService
    {
        private readonly IAutoStartService _autoStartService;

        public enum PlatformType
        {
            Linux,
            Windows,
            MacOS
        }

        public Version? IgnoredVersion { get; set; }

        public bool PlatformChanged { get; private set; }

        public PlatformType Platform { get; set; } = GetDefaultPlatform();

        public string DefaultCulture { get; set; } = CultureInfo.CurrentUICulture.Name;

        public int DefaultTheme { get; set; }

        /// <summary>
        /// 程序关闭是否显示对话框
        /// </summary>
        public bool NeedExitDialogOnClose { get; set; }

        /// <summary>
        /// 隐藏关闭时的图标
        /// </summary>
        public bool HideTrayIconOnClose { get; set; }

        /// <summary>
        /// 开机自动打开程序
        /// </summary>
        public bool AutoStartEnabled { get; set; }

        public SettingService(IAutoStartService autoStartService)
        {
            _autoStartService = autoStartService;
        }

        private static PlatformType GetDefaultPlatform()
        {
            if (OperatingSystem.IsLinux())
                return PlatformType.Linux;
            if (OperatingSystem.IsWindows())
                return PlatformType.Windows;
            if (OperatingSystem.IsMacOS())
                return PlatformType.MacOS;

            throw new NotSupportedException("Unknown platform!");
        }

        public string CurrentFontFamily { get; set; }

        public double CurrentFontSize { get; set; }

        public bool Topmost { get; set; }

        private static string ConfigPath => System.IO.Path.Combine
        (
           AppDomain.CurrentDomain.BaseDirectory,
            "appsettings.json"
        );

        public void Load()
        {
            if (!File.Exists(ConfigPath))
                return;

            Appsetting config = AppSettingsHelper.GetConfig<Appsetting>();
            DefaultCulture = config.DefaultCulture;
            DefaultTheme = config.DefaultTheme;
            HideTrayIconOnClose = config.HideTrayIconOnClose;
            NeedExitDialogOnClose = config.NeedExitDialogOnClose;
            AutoStartEnabled = _autoStartService.IsEnabled();
            CurrentFontFamily = string.IsNullOrEmpty(config.Font) ? ((FontFamily)Application.Current!.Resources[GlobalConst.FontFamilyKey]!).Name : config.Font;
            CurrentFontSize = config.FontSize ?? (double)Application.Current!.Resources[GlobalConst.FontSizeKey]!;
            Topmost = config.Topmost;
        }

        public void Save()
        {
            var config = AppSettingsHelper.GetConfig<Appsetting>();
            config.DefaultCulture = DefaultCulture;
            config.DefaultTheme = DefaultTheme;
            config.NeedExitDialogOnClose = NeedExitDialogOnClose;
            config.HideTrayIconOnClose = HideTrayIconOnClose;
            config.FontSize = CurrentFontSize;
            config.Font = CurrentFontFamily;
            config.Topmost = Topmost;
            AppSettingsHelper.Save(config);
        }

        public void SetLanguage()
        {
            Debug.Assert(Application.Current is not null);
            LocalizeExtension.ChangeLanguage(new CultureInfo(DefaultCulture));
        }

        public void ChangeAutoStart()
        {
            if (AutoStartEnabled)
                _autoStartService.Enable();
            else
                _autoStartService.Disable();
        }

        public void SetFontFamily(FontFamily fontFamily)
        {
            CurrentFontFamily = fontFamily.Name;
            Application.Current!.Resources[GlobalConst.FontFamilyKey] = fontFamily;
        }

        public void SetFontSize(double fontSize)
        {
            CurrentFontSize = fontSize;
            Application.Current!.Resources[GlobalConst.FontSizeKey] = fontSize;
        }
    }
}
