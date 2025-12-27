using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Services
{
    public sealed class WindowsAutoStartService : IAutoStartService
    {
        private const string RunKey =
            @"Software\Microsoft\Windows\CurrentVersion\Run";

        private readonly string _appName;
        private readonly string _exePath;

        public WindowsAutoStartService(string appName)
        {
            _appName = appName;
            _exePath = Process.GetCurrentProcess().MainModule!.FileName!;
        }

        public bool IsEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey);
            return key?.GetValue(_appName) != null;
        }

        public void Enable()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
            key?.SetValue(_appName, $"\"{_exePath}\"");
        }

        public void Disable()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
            key?.DeleteValue(_appName, false);
        }
    }

    public sealed class LinuxAutoStartService : IAutoStartService
    {
        private readonly string _appName;
        private readonly string _exePath;
        private readonly string _servicePath;

        public LinuxAutoStartService(string appName)
        {
            _appName = appName;
            _exePath = Process.GetCurrentProcess().MainModule!.FileName!;
            _servicePath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".config/systemd/user",
                $"{_appName}.service");
        }

        public bool IsEnabled() => File.Exists(_servicePath);

        public void Enable()
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_servicePath)!);

            File.WriteAllText(_servicePath, $"""
        [Unit]
        Description={_appName}

        [Service]
        ExecStart={_exePath}

        [Install]
        WantedBy=default.target
        """);

            Run("systemctl", "--user daemon-reload");
            Run("systemctl", $"--user enable {_appName}");
        }

        public void Disable()
        {
            Run("systemctl", $"--user disable {_appName}");
            if (File.Exists(_servicePath))
                File.Delete(_servicePath);
        }

        private static void Run(string file, string args)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = file,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
        }
    }

    public sealed class MacAutoStartService : IAutoStartService
    {
        private readonly string _plistPath;
        private readonly string _appName;
        private readonly string _exePath;

        public MacAutoStartService(string appName)
        {
            _appName = appName;
            _exePath = Process.GetCurrentProcess().MainModule!.FileName!;
            _plistPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "Library/LaunchAgents",
                $"com.{_appName}.plist");
        }

        public bool IsEnabled() => File.Exists(_plistPath);

        public void Enable()
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_plistPath)!);

            File.WriteAllText(_plistPath, $"""
        <?xml version="1.0" encoding="UTF-8"?>
        <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN"
         "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
        <plist version="1.0">
        <dict>
            <key>Label</key>
            <string>com.{_appName}</string>
            <key>ProgramArguments</key>
            <array>
                <string>{_exePath}</string>
            </array>
            <key>RunAtLoad</key>
            <true/>
        </dict>
        </plist>
        """);

            Run("launchctl", $"load {_plistPath}");
        }

        public void Disable()
        {
            Run("launchctl", $"unload {_plistPath}");
            if (File.Exists(_plistPath))
                File.Delete(_plistPath);
        }

        private static void Run(string file, string args)
        {
            Process.Start(file, args);
        }
    }

}
