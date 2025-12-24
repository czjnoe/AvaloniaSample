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

        public bool AutoRemoveDeps { get; }

        public bool RequiresWorkaroundClient { get; set; }

        public GamePlatform Platform { get; set; } = GetDefaultPlatform();

        public string PreferredCulture { get; set; } = CultureInfo.CurrentUICulture.Name;

        public Theme PreferredTheme { get; set; } = Theme.Dark;

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
                res?.DetectLinuxGamePlatform();

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
            settings.DetectLinuxGamePlatform();

            settings.Save();

            return settings;
        }

        /// <summary>
        /// If the users is using proton, swap the platform to Windows and vice versa.
        ///
        /// Called on both creation and loading of settings in case
        /// a user swaps after a previous use of the application.
        /// </summary>
        private void DetectLinuxGamePlatform()
        {
            if (GetDefaultPlatform() != GamePlatform.Linux)
                return;

            var prev = Platform;

            try
            {
                string @base = System.IO.Path.GetFullPath(System.IO.Path.Combine(ManagedFolder, "..", ".."));

                Platform = File.Exists(System.IO.Path.Combine(@base, "hollow_knight.exe"))
                    // We're on proton.
                    ? GamePlatform.Windows
                    // Native
                    : GamePlatform.Linux;

                Log.Logger.Debug("Platform detected: {Platform}", Platform);

                if (prev != Platform)
                {
                    Log.Logger.Information("Platform changed from {Prev} to {Platform}, forcing re-installs!", prev, Platform);
                    PlatformChanged = true;
                }
            }
            catch (ArgumentException e)
            {
                Log.Logger.Error("Failure in path resolution for linux platform detection: {Path} with {Exception}",
                    ManagedFolder,
                    e
                );
            }
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

            Application.Current.RequestedThemeVariant = PreferredTheme == Theme.Dark
                ? ThemeVariant.Dark
                : ThemeVariant.Light;

            LocalizeExtension.ChangeLanguage(new CultureInfo(PreferredCulture));
        }
    }
}
