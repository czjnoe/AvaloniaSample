using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaloniaSample;

public partial class UserEditView : UserControl
{
    public UserEditView()
    {
        InitializeComponent();

        // ÒÆ³ýÄ¬ÈÏ±ß¿òÑùÊ½
        this.Classes.Add("no-border");
    }
}