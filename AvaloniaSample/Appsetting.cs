using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample
{
    public class Appsetting
    {
        /// <summary>
        /// 语言
        /// </summary>
        public string DefaultCulture { get; set; } = CultureInfo.CurrentUICulture.Name;

        /// <summary>
        /// 主题
        /// </summary>
        public Theme DefaultTheme { get; set; } = Theme.Dark;

        /// <summary>
        /// 程序关闭是否显示对话框
        /// </summary>
        public bool NeedExitDialogOnClose { get; set; }

        /// <summary>
        /// 隐藏关闭时的图标
        /// </summary>
        public bool HideTrayIconOnClose { get; set; }

        public string Font { get; set; }

        public double? FontSize { get; set; }
    }
}
