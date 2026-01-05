using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using AvaloniaSample.Events;
using AvaloniaSample.Models;
using AvaloniaSample.ViewModels;
using DryIoc;
using Prism.Events;
using ReactiveUI.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ursa.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AvaloniaSample.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private readonly ISettingService _settingService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;

        public MainWindow(ISettingService settingService, IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _settingService = settingService;
            _eventAggregator = eventAggregator;
            InitializeComponent();
            Init();
            _dialogService = dialogService;
        }

        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            e.Cancel = true;

            var dialogResult = Ursa.Controls.DialogResult.OK;
            if (_settingService.HideTrayIconOnClose)
            {
                if (_settingService.NeedExitDialogOnClose)
                {
                    dialogResult = await ShowOptionDialogAsync(AvaloniaSample.Resources.Resources.FindInTrayIcon,
                        DialogMode.Info,
                        DialogButton.OKCancel);
                }

                if (dialogResult != Ursa.Controls.DialogResult.OK)
                {
                    return;
                }

                Hide();
                return;
            }

            if (_settingService.NeedExitDialogOnClose)
            {
                dialogResult = await ShowOptionDialogAsync(AvaloniaSample.Resources.Resources.SureExit, DialogMode.Warning,
                    DialogButton.OKCancel);
            }

            if (dialogResult != Ursa.Controls.DialogResult.OK)
            {
                return;
            }
            Environment.Exit(0);
        }

        private async Task<Ursa.Controls.DialogResult> ShowOptionDialogAsync(string message, DialogMode mode, DialogButton button)
        {
            var options = new DialogOptions()
            {
                Title = AvaloniaSample.Resources.Resources.Exit,
                Mode = mode,
                Button = button,
                ShowInTaskBar = false,
                IsCloseButtonVisible = true,
                StartupLocation = WindowStartupLocation.CenterOwner,
                CanDragMove = false,
                CanResize = false,
                StyleClass = default,
            };
            var vm = new ExitOptionViewModel()
            {
                Message = message,
                Option = !_settingService.NeedExitDialogOnClose,
                OptionContent = AvaloniaSample.Resources.Resources.NoMorePrompts
            };
            var result = await Ursa.Controls.Dialog.ShowModal<ExitOptionView, ExitOptionViewModel>(vm, options: options);
            _settingService.NeedExitDialogOnClose = !vm.Option;
            _settingService.Save();
            _eventAggregator.GetEvent<ChangeNeedExitDialogOnCloseEvent>().Publish(_settingService.NeedExitDialogOnClose);
            return result;
        }

        private void Init()
        {
            _settingService.Load();
            _eventAggregator.GetEvent<ChangeApplicationStatusEvent>()
                .Subscribe(ChangeApplicationStatus);
            _eventAggregator.GetEvent<ChangeWindowTopmostEvent>()
               .Subscribe(SetWindowTopmost);
            SetWindowTopmost(_settingService.Topmost);
        }

        private void ChangeApplicationStatus(bool value)
        {
            var icon = TrayIcon.GetIcons(App.Instance)?.FirstOrDefault();
            if (icon == null)
            {
                return;
            }
            icon.IsVisible = value;
        }

        private void SetWindowTopmost(bool value)
        {
            this.Topmost = value;
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsClick(object? sender, RoutedEventArgs e)
        {
            SettingsTab.IsSelected = true;
        }

        /// <summary>
        /// 显示帮助菜单或对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHelpClick(object? sender, RoutedEventArgs e)
        {
            ContainerLocator.Container.Resolve<HelpView>().ShowDialog(App.Instance.MainWindow as Window);
        }

        /// <summary>
        /// 标题栏鼠标按下移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
        }

        private void ImageSwitchToggle_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                var button = sender as ToggleButton;
                vm.CurrentTopmost = button?.IsChecked ?? false;
            }
        }
    }
}