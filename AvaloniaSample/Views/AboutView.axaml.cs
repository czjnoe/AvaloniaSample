using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaSample.ViewModels;
using ReactiveUI.Avalonia;

namespace AvaloniaSample.Views;

public partial class AboutView : ReactiveUserControl<AboutViewModel>
{
    public AboutView()
    {
        InitializeComponent();
    }
}