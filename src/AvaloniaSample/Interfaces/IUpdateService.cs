using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace AvaloniaSample.Interfaces
{
    public interface IUpdateService
    {
        /// <summary>
        /// 获取当前版本
        /// </summary>
        string GetCurrentVersion();

        /// <summary>
        /// 检查是否有更新
        /// </summary>
        Task<UpdateInfo?> CheckForUpdatesAsync();

        /// <summary>
        /// 下载更新
        /// </summary>
        Task<bool> DownloadUpdatesAsync(
          UpdateInfo updateInfo,
          Action<int>? progressCallback = null);

        /// <summary>
        /// 应用更新并重启
        /// </summary>
        void ApplyUpdatesAndRestart(UpdateInfo updateInfo);

        /// <summary>
        /// 仅应用更新（不重启）
        /// </summary>
        void ApplyUpdatesAndExit(UpdateInfo updateInfo);
    }
}
