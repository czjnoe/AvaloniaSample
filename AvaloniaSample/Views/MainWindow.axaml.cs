using Avalonia.Controls;
using AvaloniaSample.Models;
using AvaloniaSample.ViewModels;
using ReactiveUI.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaloniaSample.Views
{
    public partial class MainWindow : ReactiveWindow<UserEditViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}