using AvaloniaSample.RegionAdapters;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.ViewModels
{
    public class UpdateLogViewModel : ViewModelBase, ITabItemBase
    {
        public UpdateLogViewModel()
        {
            var path = "UpdateLog.md";
            if (File.Exists(path))
            {
                UpdateLogMarkdownContent = File.ReadAllText(path);
            }
            else
            {
                UpdateLogMarkdownContent = "Empty";
            }
        }

        public string? TitleKey { get; set; } = Resources.Resources.Tab_UpdateLog_Title;
        public string? MessageKey { get; set; } = Resources.Resources.Tab_UpdateLog_Description;

        private string? _updateLogMarkdownContent;

        public string? UpdateLogMarkdownContent
        {
            get => _updateLogMarkdownContent;
            set => this.RaiseAndSetIfChanged(ref _updateLogMarkdownContent, value);
        }
    }
}
