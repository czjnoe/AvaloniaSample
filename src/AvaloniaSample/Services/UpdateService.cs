using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace AvaloniaSample.Services
{
    public class UpdateService
    {
        private readonly UpdateManager? _updateManager;
        private readonly string _updateUrl;

        public UpdateService(string updateUrl)
        {
            _updateUrl = updateUrl;

            try
            {
                // 创建更新管理器
                _updateManager = new UpdateManager(
                    new GithubSource(_updateUrl, null, false));

                // 或使用自定义服务器
                // _updateManager = new UpdateManager(
                //     new SimpleWebSource(_updateUrl));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"初始化更新管理器失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取当前版本
        /// </summary>
        public string GetCurrentVersion()
        {
            try
            {
                return _updateManager?.CurrentVersion?.ToString() ?? "1.0.0";
            }
            catch
            {
                return "1.0.0";
            }
        }

        /// <summary>
        /// 检查是否有更新
        /// </summary>
        public async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            if (_updateManager == null)
                return null;

            try
            {
                var updateInfo = await _updateManager.CheckForUpdatesAsync();
                return updateInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"检查更新失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 下载更新
        /// </summary>
        public async Task<bool> DownloadUpdatesAsync(
            UpdateInfo updateInfo,
            Action<int>? progressCallback = null)
        {
            if (_updateManager == null)
                return false;

            try
            {
                await _updateManager.DownloadUpdatesAsync(
                    updateInfo,
                    progress => progressCallback?.Invoke(progress));

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"下载更新失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 应用更新并重启
        /// </summary>
        public void ApplyUpdatesAndRestart(UpdateInfo updateInfo)
        {
            try
            {
                _updateManager?.ApplyUpdatesAndRestart(updateInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"应用更新失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 仅应用更新（不重启）
        /// </summary>
        public void ApplyUpdatesAndExit(UpdateInfo updateInfo)
        {
            try
            {
                _updateManager?.ApplyUpdatesAndExit(updateInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"应用更新失败: {ex.Message}");
            }
        }
    }
}
