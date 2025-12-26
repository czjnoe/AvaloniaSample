using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
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

        bool AutoOpenToolboxAtStartup { get; set; }

        void Load();

        void Save();

        void Apply();
    }
}
