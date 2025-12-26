using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI.Avalonia;

namespace AvaloniaSample.Views;

public partial class ExitOptionView : ReactiveUserControl<ExitOptionViewModel>
{
    public ExitOptionView()
    {
        InitializeComponent();
    }
}