using AvaloniaSample.Helper;

namespace AvaloniaSample.Services
{
    public class Settings : ISettings
    {
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
        public bool AutoOpenToolboxAtStartup { get; set; }

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
            AutoOpenToolboxAtStartup = config.AutoOpenToolboxAtStartup;
        }

        public void Save()
        {
            var config = AppSettingsHelper.GetConfig<Appsetting>();
            config.DefaultCulture = DefaultCulture;
            config.DefaultTheme = DefaultTheme;
            config.NeedExitDialogOnClose = NeedExitDialogOnClose;
            config.HideTrayIconOnClose = HideTrayIconOnClose;
            config.AutoOpenToolboxAtStartup = AutoOpenToolboxAtStartup;
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
    }
}
