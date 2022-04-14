using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OneOuchn
{
    /// <summary>
    /// 浏览器帮助
    /// </summary>
    public class BrowerHelper
    {
        public Browser Browser { get; set; }

        public Page MainPage { get; set; }

        /// <summary>
        /// 启动浏览器
        /// </summary>
        /// <returns></returns>
        public async Task StartUpBrowser()
        {
            await CheckDownloadBrowser();

            var extra = new PuppeteerExtra()
                .Use(new StealthPlugin());

            try
            {
                Browser = await extra.LaunchAsync(new LaunchOptions()
                {
                    Headless = true,
                    IgnoredDefaultArgs = new string[] { "--incognito" }
                });
                var Pages = await Browser.PagesAsync();
                MainPage = Pages[0];
            }
            catch
            {
                LogHelper.WriteErrorLine($"连接到浏览器失败,请按照教程再启动一遍浏览器试试...");
                Environment.Exit(-1);
            }
            LogHelper.WriteSuccessLine($"启动浏览器成功!");
        }

        /// <summary>
        /// 检查下载浏览器
        /// </summary>
        /// <returns></returns>
        public async static Task CheckDownloadBrowser()
        {
            Console.WriteLine("----------------------------------------------------------------------");
            var br = new BrowserFetcher();
            var ExecutablePath = await br.GetRevisionInfoAsync();

            var Downloaded = ExecutablePath.Downloaded;
            Console.WriteLine($"检查当前是否下载浏览器:{(Downloaded == true ? "已下载" : "未下载")}");

            if (!Downloaded)
            {
                Console.WriteLine($"准备开始下载浏览器");
                //Console.WriteLine($"平台:{ExecutablePath.Platform}");
                //Console.WriteLine($"当前默认选择浏览器:{br.Product}");
                //Console.WriteLine($"默认下载浏览器版本:{ExecutablePath.Revision}");

                /*Console.WriteLine($"Local:{ExecutablePath.Local}");
                Console.WriteLine($"文件夹路径:{ExecutablePath.FolderPath}");
                Console.WriteLine($"执行文件路径:{ExecutablePath.ExecutablePath}");
                Console.WriteLine($"手动下载地址:{ExecutablePath.Url}");*/

                LogHelper.WriteSuccessLine($"正在下载浏览器,因浏览器较大请耐心等待(大概100M)....");

                br.DownloadProgressChanged += (a, b) => //下载进度条事件
                {
                    if (b.ProgressPercentage >= 100)
                    {
                        LogHelper.WriteSuccessLine("下载浏览器完毕");
                    }
                };

                await br.DownloadAsync();
            }
            else
            {
                //Console.WriteLine($"平台:{ExecutablePath.Platform}");
                //Console.WriteLine($"当前浏览器:{br.Product}");
                //Console.WriteLine($"当前浏览器版本:{ExecutablePath.Revision}");
            }
            var sb = new StringBuilder();
            sb.AppendLine($"OS:{RuntimeInformation.RuntimeIdentifier}");
            sb.AppendLine($"OS Version：{Environment.OSVersion}");
            sb.AppendLine($"OS Architecture:{RuntimeInformation.OSArchitecture}");
            sb.AppendLine($"Framework Description:{RuntimeInformation.FrameworkDescription}");
            sb.AppendLine($"MachineName：{Environment.MachineName}");
            /*sb.AppendLine($"CommandLine：{ Environment.CommandLine}");
            sb.AppendLine($"CurrentDirectory：{ Environment.CurrentDirectory}");
            sb.AppendLine($"SystemDirectory: { Environment.SystemDirectory}");*/

            Console.WriteLine(sb.ToString());
            Console.WriteLine("----------------------------------------------------------------------");
        }
    }
}
