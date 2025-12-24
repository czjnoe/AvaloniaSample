using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Prism.DryIoc.Properties;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.ViewModels
{
    public partial class SettingsViewModel : ViewModelBase
    {
        public ISettings Settings { get; }

        public static string[] Languages => new[]
        {
        "en-US",
        "zh-CN"
    };

        public static ThemeVariant[] Themes => new[]
        {
        ThemeVariant.Dark,
        ThemeVariant.Light
    };

        private ThemeVariant _theme;

        public ThemeVariant Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                RaisePropertyChanged("Theme");
            }
        }

        private string _selected;

        public string Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                RaisePropertyChanged("Selected");
            }
        }

        public SettingsViewModel(ISettings settings)
        {
            Settings = settings;

            Selected = settings.PreferredCulture;

            _theme = settings.PreferredTheme == Enums.Theme.Dark
                ? ThemeVariant.Dark
                : ThemeVariant.Light;

            this.ObservableForProperty(x => x.Selected)
                .Subscribe(o =>
                {
                    var item = o.GetValue();

                    if (string.IsNullOrEmpty(item))
                        return;

                    settings.PreferredCulture = item;

                    settings.Apply();
                    settings.Save();
                });

            this.ObservableForProperty(x => x.Theme)
                .Subscribe(
                    o =>
                    {
                        var t = o.GetValue();

                        Application.Current!.RequestedThemeVariant = t;
                        settings.PreferredTheme = t == ThemeVariant.Dark
                            ? AvaloniaSample.Enums.Theme.Dark
                            : AvaloniaSample.Enums.Theme.Light;

                        settings.Save();
                    }
                );
        }
    }
}
