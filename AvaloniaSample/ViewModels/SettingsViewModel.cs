using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using AvaloniaSample.Events;
using Prism.DryIoc.Properties;
using Prism.Events;
using ReactiveUI;
using Semi.Avalonia;
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
        private readonly IEventAggregator _eventAggregator;

        public static string[] Languages => new[]
        {
        "en-US",
        "zh-CN"
    };

        public static ThemeVariant[] Themes => new[]
        {
        ThemeVariant.Dark,
        ThemeVariant.Light,
        SemiTheme.Aquatic,
        SemiTheme.Desert,
        SemiTheme.Dusk,
        SemiTheme.NightSky
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

        private bool _hideTrayIconOnClose;

        public bool HideTrayIconOnClose
        {
            get => _hideTrayIconOnClose;
            set => this.RaiseAndSetIfChanged(ref _hideTrayIconOnClose, value);
        }

        private bool _needExitDialogOnClose;

        public bool NeedExitDialogOnClose
        {
            get => _needExitDialogOnClose;
            set => this.RaiseAndSetIfChanged(ref _needExitDialogOnClose, value);
        }

        private bool _autoOpenToolboxAtStartup;

        public bool AutoOpenToolboxAtStartup
        {
            get => _autoOpenToolboxAtStartup;
            set => this.RaiseAndSetIfChanged(ref _autoOpenToolboxAtStartup, value);
        }

        public SettingsViewModel(ISettings settings, IEventAggregator eventAggregator)
        {
            Settings = settings;
            _eventAggregator = eventAggregator;
            Selected = settings.DefaultCulture;
            HideTrayIconOnClose = settings.HideTrayIconOnClose;
            NeedExitDialogOnClose = settings.NeedExitDialogOnClose;
            AutoOpenToolboxAtStartup = settings.AutoOpenToolboxAtStartup;

            _theme = settings.DefaultTheme == Enums.Theme.Dark
                ? ThemeVariant.Dark
                : ThemeVariant.Light;

            this.ObservableForProperty(x => x.Selected)
                .Subscribe(o =>
                {
                    var item = o.GetValue();

                    if (string.IsNullOrEmpty(item))
                        return;

                    settings.DefaultCulture = item;

                    settings.Apply();
                    settings.Save();
                });//订阅 Selected 修改

            this.ObservableForProperty(x => x.Theme)
                .Subscribe(
                    o =>
                    {
                        var t = o.GetValue();

                        Application.Current!.RequestedThemeVariant = t;
                        settings.DefaultTheme = t == ThemeVariant.Dark
                            ? AvaloniaSample.Enums.Theme.Dark
                            : AvaloniaSample.Enums.Theme.Light;

                        settings.Save();
                    }
                );//订阅 Theme 修改

            this.ObservableForProperty(x => x.AutoOpenToolboxAtStartup)
               .Subscribe(o =>
               {
                   var value = o.GetValue();
                   settings.AutoOpenToolboxAtStartup = value;
                   settings.Save();
               });//订阅 AutoOpenToolboxAtStartup 修改
        }

        public void ChangeHideTrayIconOnCloseHandler()
        {
            Settings.HideTrayIconOnClose = HideTrayIconOnClose;
            Settings.Save();
            _eventAggregator.GetEvent<ChangeApplicationStatusEvent>().Publish(HideTrayIconOnClose);
        }

        public void ChangeDisplayPromptWhenClosingHandler()
        {
            Settings.NeedExitDialogOnClose = NeedExitDialogOnClose;
            Settings.Save();
        }
    }
}
