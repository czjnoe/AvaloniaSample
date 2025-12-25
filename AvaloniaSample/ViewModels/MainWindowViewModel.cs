using AvaloniaSample.Models;
using AvaloniaSample.Services;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AvaloniaSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public SettingsViewModel SettingsViewModel { get; }
        public UserOperateViewModel UserOperateViewModel { get; }

        public MainWindowViewModel(SettingsViewModel settingsViewModel, UserOperateViewModel userOperateViewModel)
        {
            SettingsViewModel = settingsViewModel;
            UserOperateViewModel = userOperateViewModel;
        }
    }
}
