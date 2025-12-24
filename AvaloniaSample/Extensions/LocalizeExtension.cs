using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Prism.DryIoc.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Extensions
{
    internal sealed class LocalizeExtension : MarkupExtension, INotifyPropertyChanged
    {
        public static void ChangeLanguage(CultureInfo ci)
        {
            Resources.Resources.Culture = ci;
            OnLanguageChanged?.Invoke();
        }

        private static event Action? OnLanguageChanged;

        private string Key { get; }

        public string Result =>
            Resources.Resources.ResourceManager.GetString(Key, Resources.Resources.Culture)?.Replace("\\n", "\n") ?? $"#{Key}#";

        public LocalizeExtension(string key)
        {
            Key = key;

            OnLanguageChanged += () => {
                OnPropertyChanged(nameof(Result));
            };
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding(nameof(Result)) { Source = this };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
