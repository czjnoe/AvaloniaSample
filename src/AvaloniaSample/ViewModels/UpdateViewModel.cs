using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Velopack;

namespace AvaloniaSample.ViewModels
{
    public class UpdateViewModel : ViewModelBase
    {
        private readonly IUpdateService _updateService;
        private UpdateInfo? _updateInfo;

        private UpdateStatus _status = UpdateStatus.Idle;
        private string _statusMessage = "就绪";
        private string _currentVersion;
        private string _latestVersion = "";
        private int _downloadProgress;
        private string _errorMessage = "";
        private bool _showUpdateButton;
        private bool _showDownloadButton;
        private bool _showInstallButton;

        public UpdateViewModel()
        {
            try
            {
                _updateService = ContainerLocator.Container.Resolve<IUpdateService>();
                _currentVersion = _updateService.GetCurrentVersion();
            }
            catch (Exception)
            {
                // 如果依赖注入失败，使用默认值
                _currentVersion = "1.0.0";
            }

            // 初始化按钮可见性
            UpdateButtonVisibility();

            // 命令初始化
            CheckUpdateCommand = ReactiveCommand.CreateFromTask(CheckForUpdatesAsync);
            DownloadUpdateCommand = ReactiveCommand.CreateFromTask(
                DownloadUpdatesAsync,
                this.WhenAnyValue(x => x.ShowDownloadButton));
            InstallUpdateCommand = ReactiveCommand.Create(
                InstallUpdate,
                this.WhenAnyValue(x => x.ShowInstallButton));
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        #region Properties

        public UpdateStatus Status
        {
            get => _status;
            set
            {
                this.RaiseAndSetIfChanged(ref _status, value);
                UpdateButtonVisibility();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public string CurrentVersion
        {
            get => _currentVersion;
            set => this.RaiseAndSetIfChanged(ref _currentVersion, value);
        }

        public string LatestVersion
        {
            get => _latestVersion;
            set => this.RaiseAndSetIfChanged(ref _latestVersion, value);
        }

        public int DownloadProgress
        {
            get => _downloadProgress;
            set => this.RaiseAndSetIfChanged(ref _downloadProgress, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public bool ShowUpdateButton
        {
            get => _showUpdateButton;
            set => this.RaiseAndSetIfChanged(ref _showUpdateButton, value);
        }

        public bool ShowDownloadButton
        {
            get => _showDownloadButton;
            set => this.RaiseAndSetIfChanged(ref _showDownloadButton, value);
        }

        public bool ShowInstallButton
        {
            get => _showInstallButton;
            set => this.RaiseAndSetIfChanged(ref _showInstallButton, value);
        }

        #endregion

        #region Commands

        public ReactiveCommand<Unit, Unit> CheckUpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> DownloadUpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> InstallUpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 检查更新
        /// </summary>
        public async Task CheckForUpdatesAsync()
        {
            try
            {
                Status = UpdateStatus.Checking;
                StatusMessage = "正在检查更新...";
                ErrorMessage = "";

                _updateInfo = await _updateService.CheckForUpdatesAsync();

                if (_updateInfo == null)
                {
                    Status = UpdateStatus.NoUpdate;
                    StatusMessage = "当前已是最新版本";
                    await Task.Delay(2000);

                    if (Status == UpdateStatus.NoUpdate)
                    {
                        Status = UpdateStatus.Idle;
                        StatusMessage = "就绪";
                    }
                }
                else
                {
                    Status = UpdateStatus.UpdateAvailable;
                    LatestVersion = _updateInfo.TargetFullRelease.Version.ToString();
                    StatusMessage = $"发现新版本 {LatestVersion}";
                }
            }
            catch (Exception ex)
            {
                Status = UpdateStatus.Error;
                StatusMessage = "检查更新失败";
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// 下载更新
        /// </summary>
        private async Task DownloadUpdatesAsync()
        {
            if (_updateInfo == null)
                return;

            try
            {
                Status = UpdateStatus.Downloading;
                StatusMessage = "正在下载更新...";
                DownloadProgress = 0;

                var success = await _updateService.DownloadUpdatesAsync(
                    _updateInfo,
                    progress =>
                    {
                        DownloadProgress = progress;
                        StatusMessage = $"正在下载更新... {progress}%";
                    });

                if (success)
                {
                    Status = UpdateStatus.ReadyToInstall;
                    StatusMessage = "更新已下载完成，可以安装";
                    DownloadProgress = 100;
                }
                else
                {
                    Status = UpdateStatus.Error;
                    StatusMessage = "下载失败";
                    ErrorMessage = "更新下载失败，请重试";
                }
            }
            catch (Exception ex)
            {
                Status = UpdateStatus.Error;
                StatusMessage = "下载失败";
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// 安装更新
        /// </summary>
        private void InstallUpdate()
        {
            if (_updateInfo == null)
                return;

            try
            {
                Status = UpdateStatus.Installing;
                StatusMessage = "正在安装更新并重启...";

                // 延迟一下让用户看到消息
                Task.Delay(1000).ContinueWith(_ =>
                {
                    _updateService.ApplyUpdatesAndRestart(_updateInfo);
                });
            }
            catch (Exception ex)
            {
                Status = UpdateStatus.Error;
                StatusMessage = "安装失败";
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        private void Cancel()
        {
            Status = UpdateStatus.Idle;
            StatusMessage = "就绪";
            ErrorMessage = "";
            DownloadProgress = 0;
        }

        /// <summary>
        /// 更新按钮可见性
        /// </summary>
        private void UpdateButtonVisibility()
        {
            ShowUpdateButton = Status == UpdateStatus.Idle || Status == UpdateStatus.NoUpdate || Status == UpdateStatus.Error;
            ShowDownloadButton = Status == UpdateStatus.UpdateAvailable;
            ShowInstallButton = Status == UpdateStatus.ReadyToInstall;
        }

        /// <summary>
        /// 启动时静默检查更新
        /// </summary>
        public async Task CheckForUpdatesOnStartupAsync()
        {
            await Task.Delay(2000); // 延迟2秒
            await CheckForUpdatesAsync();
        }

        #endregion
    }
}
