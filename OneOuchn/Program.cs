using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PuppeteerSharp;
using RestSharp;
using System.Text;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerExtraSharp;
using YamlDotNet.Serialization;
using Spectre.Console;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace OneOuchn;

/// <summary>
/// 
/// </summary>
public static class Program
{
    #region 公共变量
   // public static Logger Loggerr { get; set; } = new LoggerConfiguration()
   //.MinimumLevel.Debug()
   //.WriteTo.Console()
   //.WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log-") + ".txt", rollingInterval: RollingInterval.Day)//将日志输出到目标路径，文件的生成方式为每天生成一个文件
   //.CreateLogger();

    public static BrowerHelper Browser { get; set; } = new BrowerHelper();

    public static OneOuchnHelper OneOuchnHelper { get; set; }

    public static ConfigureHelper ConfigureHelper = new ConfigureHelper();
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static async Task Main()
    {
        var sw = new Stopwatch();
        try
        {
            var Info = @"
    _       __    _                __    ______                  
   | |     / /   (_)   ____   ____/ /   / ____/   ____ _   ____ _
   | | /| / /   / /   / __ \ / __  /   / /_      / __ `/  / __ `/
   | |/ |/ /   / /   / / / // /_/ /   / __/     / /_/ /  / /_/ / 
   |__/|__/   /_/   /_/ /_/ \__,_/   /_/        \__, /   \__, /  
                                               /____/   /____/   

                                          --国开一网一平台自动刷课
                                          --博客:https://windfgg.github.io/";

            LogHelper.WriteSuccessLine(Info, Color.Green);
        
            OneOuchnHelper = new OneOuchnHelper(Browser, ConfigureHelper);

            if (!ConfigureHelper.Configure.CookieLogin)
            {
                await Browser.StartUpBrowser();
                await OneOuchnHelper.LoginOuchn(); //登录国开
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(ConfigureHelper.Configure.Cookie))
                {
                    if (await OneOuchnHelper.CookieCheck())
                    {
                        LogHelper.WriteErrorLine($"Cookie已过期请重新打开程序登录(输入任意键退出)");
                        ConfigureHelper.SetCookieLogin(false);
                        Console.ReadKey();
                        Environment.Exit(-1);
                    }
                }
                else
                {
                    LogHelper.WriteErrorLine($"Cookie为空请重新打开程序登录(输入任意键退出)");
                    ConfigureHelper.SetCookieLogin(false);
                    Console.ReadKey();
                    Environment.Exit(-1);
                }
                
            }
            foreach (var item in OneOuchnHelper.CourseList)
            {
                LogHelper.WriteSuccessLine($"\n===========开始学习课程：{item.Key}({item.Value})===========\n");
                sw.Start();
                LogHelper.WriteSuccessLine($"\n===========开始计时===========\n");
                await OneOuchnHelper.LearnCourseId(item.Value);
                sw.Stop();
                LogHelper.WriteSuccessLine($"\n===========该课程共计用时：{sw.Elapsed}===========\n");
                LogHelper.WriteErrorLine($"\n===========课程：{item.Key}({item.Value})学习结束===========\n");
                sw = new Stopwatch();
            }

            LogHelper.WriteSuccessLine($"学习完毕,本次学习耗时:{sw.Elapsed}");
            Console.WriteLine("请按下任意键退出...");
            Console.ReadKey();
        }

        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            sw.Stop();
            LogHelper.WriteErrorLine("发生错误...");
            LogHelper.WriteSuccessLine($"本次学习耗时:{sw.Elapsed}");
        }
        finally
        {
            if (Browser.Browser != null)
            {
                Browser.Browser.Dispose();
            }
        }
    }

}