using AvaloniaSample.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Modules
{
    public class HelpModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager? regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion<UpdateLogView>(RegionNames.HelpRegion);
            regionManager.RegisterViewWithRegion<AboutView>(RegionNames.HelpRegion);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<UpdateLogView>();
            containerRegistry.Register<UpdateLogViewModel>();
            containerRegistry.Register<AboutView>();
            containerRegistry.Register<AboutViewModel>();
        }
    }
}
