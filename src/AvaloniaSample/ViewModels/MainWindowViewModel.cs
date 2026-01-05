using Avalonia.Interactivity;
using AvaloniaSample.Events;
using AvaloniaSample.Models;
using AvaloniaSample.Services;
using Prism.Commands;
using Prism.Events;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AvaloniaSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public SettingsViewModel SettingsViewModel { get; }
        public UserOperateViewModel UserOperateViewModel { get; }
        private readonly ISettingService _settingService;
        private readonly IEventAggregator _eventAggregator;

        private bool currentTopmost;
        public bool CurrentTopmost
        {
            get => currentTopmost;
            set
            {
                this.RaiseAndSetIfChanged(ref currentTopmost, value);
                ChangeWindowTopmostHandler();
            }
        }

        public MainWindowViewModel(SettingsViewModel settingsViewModel, UserOperateViewModel userOperateViewModel, ISettingService settingService, IEventAggregator eventAggregator)
        {
            SettingsViewModel = settingsViewModel;
            UserOperateViewModel = userOperateViewModel;
            _settingService = settingService;
            _eventAggregator = eventAggregator;
            Init();
        }

        private void Init()
        {
            CurrentTopmost = _settingService.Topmost;
        }

        public void ChangeWindowTopmostHandler()
        {
            _eventAggregator.GetEvent<ChangeWindowTopmostEvent>().Publish(CurrentTopmost);
            _settingService.Topmost = CurrentTopmost;
            _settingService.Save();
        }
    }
}
