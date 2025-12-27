using Avalonia.Media;
using AvaloniaSample.Consts;
using AvaloniaSample.Helper;
using AvaloniaSample.Interfaces;
using System.Globalization;

namespace AvaloniaSample.Services
{
    public class Settings : ISettings
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

        public Theme DefaultTheme { get; set; } = Theme.Dark;

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

        public Settings(IAutoStartService autoStartService)
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
            HideTrayIconOnClose = config.HideTrayIconOnClose;
            NeedExitDialogOnClose = config.NeedExitDialogOnClose;
            AutoStartEnabled = _autoStartService.IsEnabled();
            CurrentFontFamily = string.IsNullOrEmpty(config.Font) ? ((FontFamily)Application.Current!.Resources[GlobalConst.FontFamilyKey]!).Name : config.Font;
            CurrentFontSize = config.FontSize ?? (double)Application.Current!.Resources[GlobalConst.FontSizeKey]!;
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
            AppSettingsHelper.Save(config);
        }

        public void Apply()
        {
            Debug.Assert(Application.Current is not null);

            Application.Current.RequestedThemeVariant = DefaultTheme == Theme.Dark
                ? ThemeVariant.Dark
                : ThemeVariant.Light;

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
            Application.Current!.Resources[GlobalConst.FontFamilyKey] = fontFamily;
        }

        public void SetFontSize(double fontSize)
        {
            Application.Current!.Resources[GlobalConst.FontSizeKey] = fontSize;
        }
    }
}
