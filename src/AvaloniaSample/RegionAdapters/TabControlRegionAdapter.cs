using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.RegionAdapters
{
    public class TabControlRegionAdapter : RegionAdapterBase<TabControl>
    {
        public TabControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, TabControl regionTarget)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            regionTarget.SelectionChanged += (s, e) =>
            {
                if (regionTarget.SelectedItem is TabItem { Content: UserControl { DataContext: ITabItemBase vm } })
                {
                    regionTarget.Tag = vm.MessageKey;
                }
            };

            // 监听 Region 中视图的变化
            region.Views.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (e.NewItems != null)
                        {
                            foreach (var item in e.NewItems)
                            {

                                string? header = null; // 获取 Tab 的标题

                                if (item is UserControl { DataContext: ITabItemBase tabItem })
                                {
                                    header = tabItem.TitleKey;
                                } // 如果 View 实现了 ITabItemBase 接口
                                else
                                {
                                    header = item?.GetType().Name.Replace("View", "");
                                }  // 使用 View 的类型名称

                                var newTabItem = new TabItem
                                {
                                    Header = header,
                                    Content = item
                                };

                                regionTarget.Items.Add(newTabItem);
                            }

                            // 默认选中第一个 Tab
                            if (regionTarget.SelectedIndex == -1 && regionTarget.Items.Count > 0)
                            {
                                regionTarget.SelectedIndex = 0;
                            }
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems != null)
                        {
                            foreach (var item in e.OldItems)
                            {
                                var tabToDelete = regionTarget.Items.OfType<TabItem>()
                                    .FirstOrDefault(n => n.Content == item);
                                regionTarget.Items.Remove(tabToDelete);
                            }
                        }
                        break;
                }
            };
        }

        protected override IRegion CreateRegion() => new SingleActiveRegion();
    }

    public interface ITabItemBase
    {
        public string? TitleKey { get; set; }
        public string MessageKey { get; set; }
    }
}
