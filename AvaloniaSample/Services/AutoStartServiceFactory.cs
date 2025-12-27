using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Services
{
    public static class AutoStartServiceFactory
    {
        public static IAutoStartService Create(string appName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsAutoStartService(appName);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return new LinuxAutoStartService(appName);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new MacAutoStartService(appName);

            throw new PlatformNotSupportedException();
        }
    }
}
