namespace AvaloniaSample.Services
{
    public class Settings : ISettings
    {
        public enum GamePlatform
        {
            Linux,
            Windows,
            MacOS
        }

        public Version? IgnoredVersion { get; set; }

        public bool PlatformChanged { get; private set; }

        public string ManagedFolder { get; set; } = null!;

        public GamePlatform Platform { get; set; } = GetDefaultPlatform();

        public string DefaultCulture { get; set; } = CultureInfo.CurrentUICulture.Name;

        public Theme DefaultTheme { get; set; } = Theme.Dark;

        private static GamePlatform GetDefaultPlatform()
        {
            if (OperatingSystem.IsLinux())
                return GamePlatform.Linux;
            if (OperatingSystem.IsWindows())
                return GamePlatform.Windows;
            if (OperatingSystem.IsMacOS())
                return GamePlatform.MacOS;

            throw new NotSupportedException("Unknown platform!");
        }

        private static string ConfigPath => System.IO.Path.Combine
        (
            Environment.GetFolderPath
            (
                Environment.SpecialFolder.ApplicationData,
                Environment.SpecialFolderOption.Create
            ),
            "HKModInstaller",
            "HKInstallerSettings.json"
        );

        internal Settings(string path) : this() => ManagedFolder = path;

        // Used by serializer.
        public Settings() { }

        public static string GetOrCreateDirPath()
        {
            string dirPath = System.IO.Path.GetDirectoryName(ConfigPath) ?? throw new InvalidOperationException();

            // No-op if path already exists.
            Directory.CreateDirectory(dirPath);

            return dirPath;
        }
        public static Settings? Load()
        {
            if (!File.Exists(ConfigPath))
                return null;

            Log.Debug("ConfigPath: File @ {ConfigPath} exists.", ConfigPath);

            string content = File.ReadAllText(ConfigPath);

            try
            {
                var res = JsonSerializer.Deserialize<Settings>(content);
                return res;
            }
            // The JSON is malformed, act as if we don't have settings as a backup
            catch (Exception e) when (e is JsonException or ArgumentNullException)
            {
                return null;
            }
        }

        public static Settings Create(string path)
        {
            // Create from ManagedPath.
            var settings = new Settings(path);
            settings.Save();
            return settings;
        }

        public void Save()
        {
            string content = JsonSerializer.Serialize(this);

            GetOrCreateDirPath();

            string path = ConfigPath;

            File.WriteAllText(path, content);
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
