using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Enums
{
    public enum UpdateStatus
    {
        Idle,           // 空闲
        Checking,       // 检查中
        NoUpdate,       // 无更新
        UpdateAvailable,// 有更新
        Downloading,    // 下载中
        ReadyToInstall, // 准备安装
        Installing,     // 安装中
        Error           // 错误
    }
}
