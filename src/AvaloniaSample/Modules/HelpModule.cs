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
        /// <summary>
        /// 模块初始化时
        /// </summary>
        /// <param name="containerProvider"></param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //区域管理器（IRegionManager）：负责管理区域的生命周期、视图注入和导航。
            IRegionManager? regionManager = containerProvider.Resolve<IRegionManager>();
            //注册视图到指定区域
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
