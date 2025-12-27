using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaSample.Services;
using AvaloniaSample.ViewModels;
using AvaloniaSample.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System;

namespace AvaloniaSample
{
    /// <summary>
    /// 使用 Prism 框架的 Avalonia 应用程序的入口点。参考：https://github.com/AvaloniaCommunity/Prism.Avalonia
    /// </summary>
    public partial class App : PrismApplication
    {
        public static App Instance { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();  // Required to initialize Prism.Avalonia - DO NOT REMOVE
        }

        protected override AvaloniaObject CreateShell()
        {
            Instance = this;
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
            containerRegistry.RegisterInstance(AutoStartServiceFactory.Create("AvaloniaSample"));

            // ViewModels
            containerRegistry.Register<UserOperateViewModel>();
            containerRegistry.Register<SettingsViewModel>();
            containerRegistry.Register<MainWindowViewModel>();

            // Views
            containerRegistry.RegisterForNavigation<UserOperateView, UserOperateViewModel>();

            // Dialogs
            containerRegistry.RegisterDialog<WarningDialog, WarningDialogViewModel>("WarningDialog");
            containerRegistry.RegisterDialog<UserEditView, UserEditViewModel>("UserEditView");
            containerRegistry.RegisterDialog<UserAddView, UserAddViewModel>("UserAddView");
        }

        /// <summary>
        /// Register modules
        /// </summary>
        /// <param name="moduleCatalog"></param>
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {

            //// moduleCatalog.AddModule<DummyModule.DummyModule1>();
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApplication_OnClicked(object? sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// 显示主窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMainWindow_OnClicked(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            {
                return;
            }

            desktop.MainWindow?.Show();
            desktop.MainWindow?.Activate();
        }

        private async void OpenGithub_OnClicked(object? sender, EventArgs e)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var top = TopLevel.GetTopLevel(desktop.MainWindow);
                if (top is null) return;
                var launcher = top.Launcher;
                await launcher.LaunchUriAsync(new Uri("https://gitee.com/czjnoe/avalonia-sample.git"));

            }
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