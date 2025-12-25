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
        private readonly ISampleService _sampleService;
        //private readonly IDialogService _dialogService;

        public string Greeting { get; } = "Welcome to Avalonia!";

        public SettingsViewModel SettingsViewModel { get; }
        public UserOperateViewModel UserOperateViewModel { get; }

        //public MainWindowViewModel(ISampleService sampleService, IDialogService dialogService)
        public MainWindowViewModel(SettingsViewModel settingsViewModel, UserOperateViewModel userOperateViewModel)
        {
            SettingsViewModel = settingsViewModel;
            UserOperateViewModel = userOperateViewModel;
            //_dialogService = dialogService;
            //_sampleService = sampleService;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ShowDialog()
        {
            //var parameters = new DialogParameters
            //{
            //    { "message", "确定要删除此文件吗？" }
            //};

            //_dialogService.ShowDialog("WarningDialog", parameters, result =>
            //{
            //    if (result.Result == ButtonResult.OK)
            //    {

            //    }
            //});
        }

    }
}
