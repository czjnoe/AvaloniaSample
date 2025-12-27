using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Interfaces
{
    public interface ISettings
    {
        bool PlatformChanged { get; }

        string DefaultCulture { get; set; }

        Theme DefaultTheme { get; set; }

        bool NeedExitDialogOnClose { get; set; }

        bool HideTrayIconOnClose { get; set; }

        bool AutoStartEnabled { get; set; }

        string CurrentFontFamily { get; set; }

        double CurrentFontSize { get; set; }

        void Load();

        void Save();

        void Apply();

        void ChangeAutoStart();

        void SetFontFamily(FontFamily fontFamily);

        void SetFontSize(double fontSize);
    }
}
