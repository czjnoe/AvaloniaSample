using AvaloniaSample.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Services
{
    public static class UpdateServiceFactory
    {
        public static IUpdateService Create()
        {
            var config = AppSettingsHelper.GetConfig<Appsetting>();
            if (config.UpdateServer.Type == UpdateServerType.GitHub)
                return new GithubUpdateService();

            if (config.UpdateServer.Type == UpdateServerType.SimpleWeb)
                return new SimpleUpdateService();

            throw new PlatformNotSupportedException();
        }
    }
}
