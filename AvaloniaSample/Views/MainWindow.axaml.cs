using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaSample.Events;
using AvaloniaSample.Models;
using AvaloniaSample.ViewModels;
using Prism.Events;
using ReactiveUI.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ursa.Controls;

namespace AvaloniaSample.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private readonly ISettings _settins;
        private readonly IEventAggregator _eventAggregator;

        public MainWindow(ISettings settins, IEventAggregator eventAggregator)
        {
            _settins = settins;
            _eventAggregator = eventAggregator;
            InitializeComponent();
            Init();
        }

        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            e.Cancel = true;

            var dialogResult = Ursa.Controls.DialogResult.OK;
            if (_settins.HideTrayIconOnClose)
            {
                if (_settins.NeedExitDialogOnClose)
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

            if (_settins.NeedExitDialogOnClose)
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
                Option = !_settins.NeedExitDialogOnClose,
                OptionContent = AvaloniaSample.Resources.Resources.NoMorePrompts
            };
            var result = await Ursa.Controls.Dialog.ShowModal<ExitOptionView, ExitOptionViewModel>(vm, options: options);
            _settins.NeedExitDialogOnClose = !vm.Option;
            _settins.Save();
            return result;
        }

        private void Init()
        {
            _settins.Load();
            _eventAggregator.GetEvent<ChangeApplicationStatusEvent>()
                .Subscribe(ChangeApplicationStatus);
            _eventAggregator.GetEvent<ChangeWindowTopmostEvent>()
               .Subscribe(SetWindowTopmost);
            SetWindowTopmost(_settins.Topmost);
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
    }
}