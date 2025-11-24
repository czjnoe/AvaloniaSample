using Avalonia.Controls;
using AvaloniaSample.Models;
using AvaloniaSample.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaloniaSample.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void addBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.People.Add(new Person("New", "Person", 25));
            }
        }

        private void saveBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {

            }
        }

        private void editBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.People[0].FirstName = "Edited";
            }
        }
    }
}