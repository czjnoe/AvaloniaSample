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

        string ManagedFolder { get; set; }

        string PreferredCulture { get; set; }

        Theme PreferredTheme { get; set; }

        void Save();

        void Apply();
    }
}
