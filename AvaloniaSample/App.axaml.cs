using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaSample.Services;
using AvaloniaSample.ViewModels;
using AvaloniaSample.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;

namespace AvaloniaSample
{
    /// <summary>
    /// 使用 Prism 框架的 Avalonia 应用程序的入口点。参考：https://github.com/AvaloniaCommunity/Prism.Avalonia
    /// </summary>
    public partial class App : PrismApplication
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();  // Required to initialize Prism.Avalonia - DO NOT REMOVE
        }

        protected override AvaloniaObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// Add Services and ViewModel registrations here
        /// </summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Services
            containerRegistry.RegisterSingleton<ISampleService, SampleService>();
            containerRegistry.RegisterSingleton<ISettings, Settings>();

            // ViewModels
            containerRegistry.Register<UserOperateViewModel>();
            containerRegistry.Register<SettingsViewModel>();
            containerRegistry.Register<MainWindowViewModel>();

            // Views
            containerRegistry.RegisterForNavigation<UserOperateView, UserOperateViewModel>();

            // Dialogs
            containerRegistry.RegisterDialog<WarningDialog, WarningDialogViewModel>("WarningDialog");
            containerRegistry.RegisterDialog<UserEditView, UserEditViewModel>("UserEditView");
        }

        /// <summary>
        /// Register modules
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            
            //// moduleCatalog.AddModule<DummyModule.DummyModule1>();
        }
    }

    //public partial class App : Application
    //{
    //    public override void Initialize()
    //    {
    //        AvaloniaXamlLoader.Load(this);

    //    }

    //    public override void OnFrameworkInitializationCompleted()
    //    {
    //        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    //        {
    //            desktop.MainWindow = new MainWindow
    //            {
    //                DataContext = new MainWindowViewModel(),
    //            };
    //        }

    //        base.OnFrameworkInitializationCompleted();
    //    }
    //}
}