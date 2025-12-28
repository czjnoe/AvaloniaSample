using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaSample.Consts;
using AvaloniaSample.Services;
using AvaloniaSample.ViewModels;
using AvaloniaSample.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System;
using Velopack;

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
            // ⭐ Velopack 初始化 - 必须在最开始
            InitializeVelopack();
            AvaloniaXamlLoader.Load(this);
            base.Initialize();  // Required to initialize Prism.Avalonia - DO NOT REMOVE
        }


        protected override AvaloniaObject CreateShell()
        {
            Instance = this;
            //return Container.Resolve<MainWindow>();
            var mainWindow = Container.Resolve<MainWindow>();

            // ⭐ 订阅主窗口加载完成事件，然后检查更新
            if (mainWindow != null)
            {
                mainWindow.Opened += async (s, e) =>
                {
                    // 延迟3秒后检查更新
                    await Task.Delay(3000);
                    await CheckForUpdatesOnStartupAsync(mainWindow);
                };
            }

            return mainWindow;
        }

        #region Velopack 更新

        /// <summary>
        /// Velopack 初始化
        /// </summary>
        private void InitializeVelopack()
        {
            try
            {
                VelopackApp.Build()
                    .OnFirstRun(version =>
                    {
                        // 首次运行时的操作
                        System.Diagnostics.Debug.WriteLine($"首次运行版本: {version}");
                    })
                    .OnAfterUpdateFastCallback(version =>
                    {
                        // 更新后立即执行的操作
                        System.Diagnostics.Debug.WriteLine($"更新到版本: {version}");
                    })
                    .Run();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Velopack 初始化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 启动时检查更新
        /// </summary>
        private async Task CheckForUpdatesOnStartupAsync(Window mainWindow)
        {
            try
            {
                // 从容器解析 UpdateViewModel（如果注册了）或直接创建
                var updateViewModel = Container.IsRegistered<UpdateViewModel>()
                    ? Container.Resolve<UpdateViewModel>()
                    : new UpdateViewModel();

                // 静默检查更新
                await updateViewModel.CheckForUpdatesOnStartupAsync();

                // 如果有更新，显示更新窗口
                if (updateViewModel.Status == UpdateStatus.UpdateAvailable)
                {
                    var updateWindow = Container.IsRegistered<UpdateWindow>()
                        ? Container.Resolve<UpdateWindow>()
                        : new UpdateWindow();

                    updateWindow.DataContext = updateViewModel;
                    await updateWindow.ShowDialog(mainWindow);
                }
                // 如果检查更新出错，静默处理
                else if (updateViewModel.Status == UpdateStatus.Error)
                {
                    System.Diagnostics.Debug.WriteLine($"启动时检查更新失败: {updateViewModel.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                // 捕获所有异常，避免影响程序启动
                System.Diagnostics.Debug.WriteLine($"启动时检查更新异常: {ex.Message}");
            }
        }

        #endregion

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
            // ⭐ 注册更新服务
            containerRegistry.RegisterSingleton<UpdateService>(() =>
            {
                // 配置你的更新服务器地址
                var updateUrl = GlobalConst.GitPath + "/releases";
                return new UpdateService(updateUrl);
            });

            // ViewModels
            containerRegistry.Register<UserOperateViewModel>();
            containerRegistry.Register<SettingsViewModel>();
            containerRegistry.Register<MainWindowViewModel>();
            containerRegistry.Register<UpdateViewModel>();// 注册更新相关 ViewModel
            containerRegistry.Register<UpdateWindow>();// 注册更新窗口

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
                await launcher.LaunchUriAsync(new Uri(GlobalConst.GitPath));

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