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
        public static LocalizeExtension Instance { get; } = new();

        public static void ChangeLanguage(CultureInfo ci)
        {
            Resources.Resources.Culture = ci;
            OnLanguageChanged?.Invoke();
        }

        public static event Action? OnLanguageChanged;

        private string Key { get; }

        public string Result => GetString(Key);
        public string this[string key] => GetString(key);

        public static string GetString(string key)
        {
            return Resources.Resources.ResourceManager.GetString(key, Resources.Resources.Culture)?.Replace("\\n", "\n") ?? $"#{key}#";
        }

        private LocalizeExtension()
        {
            Key = string.Empty;
        }

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
