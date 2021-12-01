using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPuppeteerSharp
{
    /// <summary>
    /// 列举了一些ChromiumCommand  https://peter.sh/experiments/chromium-command-line-switches/ 这里基本列举了所有命令
    /// </summary>
    public static class LaunchOptionsArgs
    {
        /// <summary>
        /// 禁用拓展
        /// </summary>
        public const string disableExtensions = "--disable-extensions";

        /// <summary>
        /// 不使用沙盒
        /// </summary>
        public const string noSandbox = "--no-sandbox";

        /// <summary>
        /// 
        /// </summary>
        public const string disableSetuidSandbox = "--disable-setuid-sandbox";

        /// <summary>
        /// 隐蔽滑动条
        /// </summary>
        public const string hideScrollbars = "--hide-scrollbars";

        /// <summary>
        /// 禁用gpu
        /// </summary>
        public const string disableGpu = "--disable-gpu";

        /// <summary>
        /// 不加载图片, 提升速度，但无法显示二维码
        /// </summary>
        public const string imagesEnabled = "blink-settings=imagesEnabled=true'";

        /// <summary>
        /// 关闭声音
        /// </summary>
        public const string audioEnabled = "--mute-audio";

        /// <summary>
        /// 开启时最大化
        /// </summary>
        public const string startMaximized = "--start-maximized";

        /// <summary>
        /// 设置远程调试地址
        /// </summary>
        public const string remoteDebuggingAddress = "--remote-debugging-address=0.0.0.0";

        /// <summary>
        /// 设置远程调试端口 默认端口8888
        /// </summary>
        public const string remoteDebuggingPort = "--remote-debugging-port=8888";

        /// <summary>
        /// 设置浏览器窗口大小 1000,500
        /// </summary>
        public const string browerWindowSize = "--window-size=1000,500";
    }

    public static class IgnoredOptionsArgs
    {
        /// <summary>
        /// 防爬虫检测 去掉navigator.webdriver属性
        /// </summary>
        public const string disableExtensions = "--enable-automation";
    }
}
