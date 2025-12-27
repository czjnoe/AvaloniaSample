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
        private readonly ISettings _settings;
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

        public MainWindowViewModel(SettingsViewModel settingsViewModel, UserOperateViewModel userOperateViewModel, ISettings settings, IEventAggregator eventAggregator)
        {
            SettingsViewModel = settingsViewModel;
            UserOperateViewModel = userOperateViewModel;
            _settings = settings;
            _eventAggregator = eventAggregator;
            Init();
        }

        private void Init()
        {
            CurrentTopmost = _settings.Topmost;
        }

        public void ChangeWindowTopmostHandler()
        {
            _eventAggregator.GetEvent<ChangeWindowTopmostEvent>().Publish(CurrentTopmost);
            _settings.Topmost = CurrentTopmost;
            _settings.Save();
        }
    }
}
