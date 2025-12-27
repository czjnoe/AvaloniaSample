using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
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
        public ISettings _settings { get; }
        private readonly IEventAggregator _eventAggregator;
        private readonly IAutoStartService _autoStartService;

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

        public static IReadOnlyList<FontItem> Fonts { get; } =
        [
             new FontItem("微软雅黑", new FontFamily("Microsoft YaHei")),
             new FontItem("Segoe UI", new FontFamily("Segoe UI")),
             new FontItem("宋体", new FontFamily("SimSun")),
             new FontItem("黑体", new FontFamily("SimHei")),
             new FontItem("仿宋", new FontFamily("FangSong")),
        ];

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

        private string _selectedLanguages;

        public string SelectedLanguage
        {
            get => _selectedLanguages;
            set
            {
                _selectedLanguages = value;
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

        public bool AutoStartEnabled
        {
            get => _autoOpenToolboxAtStartup;
            set => this.RaiseAndSetIfChanged(ref _autoOpenToolboxAtStartup, value);
        }

        private FontItem? _selectedFont;
        public FontItem? SelectedFont
        {
            get => _selectedFont;
            set => this.RaiseAndSetIfChanged(ref _selectedFont, value);
        }
        public double[] FontSizes { get; } = Enumerable.Range(10, 50).Select(s => (double)s).ToArray();

        private double _selectFontSize;
        public double SelectFontSize
        {
            get => _selectFontSize;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectFontSize, value);
            }
        }

        public SettingsViewModel(ISettings settings, IEventAggregator eventAggregator, IAutoStartService autoStartService)
        {
            _settings = settings;
            _eventAggregator = eventAggregator;
            _autoStartService = autoStartService;
            SelectedLanguage = settings.DefaultCulture;
            HideTrayIconOnClose = settings.HideTrayIconOnClose;
            NeedExitDialogOnClose = settings.NeedExitDialogOnClose;
            AutoStartEnabled = settings.AutoStartEnabled;
            SelectedFont = Fonts.FirstOrDefault(s => s.FontFamily.Name == settings.CurrentFontFamily);
            SelectFontSize = settings.CurrentFontSize;
            _theme = settings.DefaultTheme == Enums.Theme.Dark
                ? ThemeVariant.Dark
                : ThemeVariant.Light;

            this.ObservableForProperty(x => x.SelectedLanguage)
                .Subscribe(o =>
                {
                    var item = o.GetValue();

                    if (string.IsNullOrEmpty(item))
                        return;

                    settings.DefaultCulture = item;

                    settings.Apply();
                    settings.Save();
                });//订阅 SelectedLanguage 修改

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

            this.ObservableForProperty(x => x.AutoStartEnabled)
               .Subscribe(o =>
               {
                   var value = o.GetValue();
                   settings.AutoStartEnabled = value;
                   settings.ChangeAutoStart();
               });//订阅 AutoStartEnabled 修改

            this.ObservableForProperty(x => x.SelectedFont)
              .Subscribe(o =>
              {
                  var value = o.GetValue();
                  settings.SetFontFamily(value.FontFamily);
                  settings.Save();
              });//订阅 SelectedFont 修改

            this.ObservableForProperty(x => x.SelectFontSize)
            .Subscribe(o =>
            {
                var value = o.GetValue();
                settings.SetFontSize(value);
                settings.Save();
            });//订阅 SelectFontSize 修改
        }

        public void ChangeHideTrayIconOnCloseHandler()
        {
            _settings.HideTrayIconOnClose = HideTrayIconOnClose;
            _settings.Save();
            _eventAggregator.GetEvent<ChangeApplicationStatusEvent>().Publish(HideTrayIconOnClose);
        }

        public void ChangeDisplayPromptWhenClosingHandler()
        {
            _settings.NeedExitDialogOnClose = NeedExitDialogOnClose;
            _settings.Save();
        }
    }
}
