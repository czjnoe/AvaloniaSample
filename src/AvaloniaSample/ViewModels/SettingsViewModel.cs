using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaSample.Events;
using AvaloniaSample.Interfaces;
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
        public ISettingService _settingService { get; }
        private readonly IEventAggregator _eventAggregator;
        private readonly IAutoStartService _autoStartService;
        private readonly IContainerProvider _container;

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
        public double[] FontSizes { get; } = Enumerable.Range(10, 30).Select(s => (double)s).ToArray();

        private double _selectFontSize;
        public double SelectFontSize
        {
            get => _selectFontSize;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectFontSize, value);
            }
        }

        private bool currentTopmost;
        public bool CurrentTopmost
        {
            get => currentTopmost;
            set
            {
                this.RaiseAndSetIfChanged(ref currentTopmost, value);
            }
        }

        public SettingsViewModel(ISettingService settingService, IEventAggregator eventAggregator, IAutoStartService autoStartService, IContainerProvider container)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _autoStartService = autoStartService;
            _eventAggregator.GetEvent<ChangeNeedExitDialogOnCloseEvent>().Subscribe(ChangeDisplayPromptWhenClosingHandler);
            var setting = ContainerLocator.Container.Resolve<ISettingService>();//容器获取对象
            setting = _container.Resolve<ISettingService>();
            _settingService = settingService;

            this.ObservableForProperty(x => x.SelectedLanguage)
                .Subscribe(o =>
                {
                    var item = o.GetValue();

                    if (string.IsNullOrEmpty(item))
                        return;

                    settingService.DefaultCulture = item;

                    settingService.SetLanguage();
                    settingService.Save();
                });//订阅 SelectedLanguage 修改

            this.ObservableForProperty(x => x.Theme)
                .Subscribe(
                    o =>
                    {
                        var t = o.GetValue();
                        Application.Current!.RequestedThemeVariant = t;
                        settingService.DefaultTheme = Themes.ToList().IndexOf(t);
                        settingService.Save();
                    }
                );//订阅 Theme 修改

            this.ObservableForProperty(x => x.AutoStartEnabled)
               .Subscribe(o =>
               {
                   var value = o.GetValue();
                   settingService.AutoStartEnabled = value;
                   settingService.ChangeAutoStart();
               });//订阅 AutoStartEnabled 修改

            this.ObservableForProperty(x => x.SelectedFont)
              .Subscribe(o =>
              {
                  var value = o.GetValue();
                  settingService.SetFontFamily(value.FontFamily);
                  settingService.Save();
              });//订阅 SelectedFont 修改

            this.ObservableForProperty(x => x.SelectFontSize)
            .Subscribe(o =>
            {
                var value = o.GetValue();
                settingService.SetFontSize(value);
                settingService.Save();
            });//订阅 SelectFontSize 修改

            Init();
        }

        private void Init()
        {
            _settingService.Load();
            SelectedLanguage = _settingService.DefaultCulture;
            HideTrayIconOnClose = _settingService.HideTrayIconOnClose;
            NeedExitDialogOnClose = _settingService.NeedExitDialogOnClose;
            AutoStartEnabled = _settingService.AutoStartEnabled;
            SelectedFont = Fonts.FirstOrDefault(s => s.FontFamily.Name == _settingService.CurrentFontFamily);
            SelectFontSize = _settingService.CurrentFontSize;
            Theme = Themes[(int)_settingService.DefaultTheme];
            CurrentTopmost = _settingService.Topmost;
        }

        public void ChangeHideTrayIconOnCloseHandler()
        {
            _settingService.HideTrayIconOnClose = HideTrayIconOnClose;
            _settingService.Save();
            _eventAggregator.GetEvent<ChangeApplicationStatusEvent>().Publish(HideTrayIconOnClose);
        }

        public void ChangeDisplayPromptWhenClosingHandler()
        {
            _settingService.NeedExitDialogOnClose = NeedExitDialogOnClose;
            _settingService.Save();
        }

        public void ChangeDisplayPromptWhenClosingHandler(bool value)
        {
            NeedExitDialogOnClose = value;
            ChangeDisplayPromptWhenClosingHandler();
        }

        public void ChangeWindowTopmostHandler()
        {
            _eventAggregator.GetEvent<ChangeWindowTopmostEvent>().Publish(CurrentTopmost);
            _settingService.Topmost = CurrentTopmost;
            _settingService.Save();
        }
    }
}
