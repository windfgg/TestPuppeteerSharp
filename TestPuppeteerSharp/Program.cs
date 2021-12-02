using PuppeteerSharp;
using PuppeteerSharp.Media;
using PushLib;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TestPuppeteerSharp
{
    /// <summary>
    /// 
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 秒表
        /// </summary>
        public static Stopwatch sw { get; set; } = new Stopwatch();

        /// <summary>
        /// 推送+推送服务
        /// </summary>
        public static PushPlus pushPlus { get; set; }

        /// <summary>
        /// 浏览器实例
        /// </summary>
        public static Browser browser { get; set; }

        /// <summary>
        /// 存储Cookies
        /// </summary>
        public static CookieParam[] cookies { get; set; }

        /// <summary>
        /// 创建浏览器实例
        /// </summary>
        /// <returns></returns>

        public async static Task<bool> CreateBrowser()
        {
            #region 本地启动
            /*            var LaunchOptions = new LaunchOptions
                        {
                            ExecutablePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                            Args = new string[]
                        {
                                                       LaunchOptionsArgs.audioEnabled
                        },
                            IgnoreDefaultArgs = true, //是否开启忽略默认参数
                            IgnoredDefaultArgs = new string[]
                        {
                                                       IgnoredOptionsArgs.disableWebdriver属性
                        },
                            Headless = false, //是否在无头模式下运行浏览器 默认true 无头
                            //Timeout = 1000 * 60,//等待浏览器实例启动的最长时间 默认30秒
                            Product = Product.Chrome,//浏览器使用哪个 默认Chrome
                            //SlowMo = 1000, //自动操作的速度非常快，以至于看不清楚元素的动作，为了方便调试，可以用 slowMo 参数放慢操作，单位 ms：
                            DefaultViewport = new ViewPortOptions() //设置默认视图
                            {
                                Width = 1920,
                                Height = 1050,
                                IsMobile = true,//是否手机
                            },
                            IgnoreHTTPSErrors = false,//导航期间是否忽略HTTPS错误。默认为false
                            Devtools = false,//是否为每个选项卡自动打开DevTools面板。如果此选项为true，则无头选项将设置为false。
                        };*/

            /*            var LaunchOptions = new LaunchOptions
                        {
                            ExecutablePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                            Headless = false, //是否在无头模式下运行浏览器 默认true 无头
                            IgnoreDefaultArgs = true, //是否开启忽略默认参数
                            IgnoredDefaultArgs = new string[]
                                    {
                                                                   IgnoredOptionsArgs.disableWebdriver属性
                                    },
                        };
                        browser = await Puppeteer.LaunchAsync(LaunchOptions);*/
            #endregion

            #region 远程链接
            //启动参数 --remote-debugging-address=0.0.0.0 --remote-debugging-port=4079
            // 检查你的浏览器远程访问是否开启:http://127.0.0.1:4079/ edge://inspect/#devices
            var ConnectOptions = new ConnectOptions()
            {
                BrowserURL = $"http://127.0.0.1:4079",
                DefaultViewport = new ViewPortOptions() //设置默认视图
                {
                    Width = 1920,
                    Height = 1028
                },
            };
            browser = await Puppeteer.ConnectAsync(ConnectOptions);
            #endregion

            return true;
        }

        /// <summary>
        /// 入口函数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main()
        {
            await CheckDownloadBrowser();
            await CreateBrowser();
            AnsiConsole.WriteLine(browser.WebSocketEndpoint); //输出ws终结点
        }

        #region 使用例子

        /// <summary>
        /// 学习强国 获取登录二维码 
        /// 踩坑1 html很多frame 需要找到登录的frame 才能通过xpath 取出登录二维码
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> GetLoginQrCode()
        {
            await using (browser)
            {
                await using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync($"https://pc.xuexi.cn/points/login.html?ref=https%3A%2F%2Fwww.xuexi.cn%2F");

                    var outerText = await page.EvaluateExpressionAsync("document.body.outerText");
                    Console.WriteLine(outerText.ToString().Contains("扫码登录") == true ? "学习强国扫码页面加载完毕" : "未获取到登录二维码");


                    Console.WriteLine($"等待三秒");
                    Thread.Sleep(1000 * 3);


                    Frame LoginFrame = null;
                    var Frames = page.Frames;
                    Console.WriteLine($"获取当前Frame总数为:{Frames.Length}个");
                    foreach (var item in Frames)
                    {
                        if (item.Name == "ddlogin-iframe")
                        {
                            LoginFrame = item;
                            Console.WriteLine($"找到ddlogin-iframe");
                            break;
                        }
                    }
                    Console.WriteLine($"钉钉登录网址:{LoginFrame.Url}");

                    var Content = await LoginFrame.GetContentAsync();
                    var login = await LoginFrame.XPathAsync(@"/html/body/div/div/div[1]/div/div[1]/div[1]/img");
                    var QrCodeSrc = await login[0].GetPropertyAsync("src");
                    //Console.WriteLine(QrCodeSrc);

                    var guid = Guid.NewGuid();

                    await pushPlus.SendMsg(@$"二维码50秒后过期</br>您的Guid为:{guid}</br>请保存二维码后到学习强国扫码</br><img src='{QrCodeSrc.ToString().Replace("JSHandle:", "")}'>", "科技强国登录");

                    Console.WriteLine($"Guid:{guid} 已发送登录推送...");
                    Console.WriteLine($"等待登录,50秒后自动退出");

                    int i = 0;
                    while (true)
                    {
                        var cookies = await page.GetCookiesAsync();
                        if (cookies != null)
                        {
                            foreach (var item in cookies)
                            {
                                if (item.Name == "token")
                                {
                                    Program.cookies = cookies;
                                    //File.AppendAllText(Json)
                                    //Console.WriteLine($"Guid:{guid}已登录");
                                    return false;
                                }
                            }
                        }
                        Thread.Sleep(1000 * 1);
                        i++;
                        Console.WriteLine($"已经过去{i}秒");
                        if (i >= 50)
                        {
                            await pushPlus.SendMsg(@$"GUID:{guid}</br><登录超时", "科技强国登录超时");
                            Console.WriteLine($"Guid:{guid} 登录超时 自动退出");
                            break;
                        }

                    }

                    Thread.Sleep(1000 * 30);

                };
            };
            Console.WriteLine("----------------------------------------------------------------------");
            return true;
        }

        /// <summary>
        /// 模拟谷歌搜索
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async Task<bool> GoogleSeach(string text)
        {
            await using (browser)
            {
                await using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync("https://www.google.com/");
                    //AnsiConsole.WriteLine("开始获取输入框焦点");
                    //await page.ClickAsync("input[id='kw']");

                    AnsiConsole.WriteLine("等待一秒");
                    await page.WaitForTimeoutAsync(1000);
                    AnsiConsole.WriteLine("模拟输入文本");
                    await page.TypeAsync("input[class='gLFyf gsfi']", text);

                    AnsiConsole.WriteLine("等待一秒");
                    await page.WaitForTimeoutAsync(1000);

                    AnsiConsole.WriteLine("模拟按下Enter键盘");
                    await page.Keyboard.PressAsync("Enter");

                    await page.WaitForNavigationAsync();  //注意 如果导航发生了变化 必须使用等待导航
                    var Content = await page.GetContentAsync(); //获取页面内容

                    var yuRUbf = await page.QuerySelectorAllAsync("div[class='yuRUbf']"); //获取class为yuRUbf所有div
                    AnsiConsole.WriteLine($"获取到前:{yuRUbf.Length}条搜索数据\n");

                    foreach (var item in yuRUbf)
                    {
                        var Tittle = await item.QuerySelectorAllAsync(@"a>h3");
                        var Url = await item.QuerySelectorAllAsync(@"a");

                        var StrTittle = await Tittle[0].GetPropertyAsync("innerText");
                        var StrUrl = await Url[0].GetPropertyAsync("href");

                        AnsiConsole.WriteLine($"标题:{StrTittle.ToString().Replace("JSHandle:", "")}\n地址:{StrUrl.ToString().Replace("JSHandle:", "")}\n\n");
                    }

                    AnsiConsole.WriteLine("等待三秒");
                    await page.WaitForTimeoutAsync(3000);
                    AnsiConsole.WriteLine("结束");
                };
            };

            return true;
        }

        /// <summary>
        /// 国开多课程简单看视频看文章
        /// </summary>
        /// <returns></returns>
        public static async void Ouchn()
        {
            var CourseList = new Dictionary<string, Page>();
            var Task = new List<Task>();
            var Pages = await browser.PagesAsync(); //获取当前所有page
            Console.WriteLine($"请按照教程来,要关闭所有的Edge界面");
            Console.WriteLine($"课程结束请输入exit退出课程,在课程未完成时也可以输入exit退出");
            Console.WriteLine($"课程结束请输入exit退出课程,在课程未完成时也可以输入exit退出");
            Console.WriteLine($"课程结束请输入exit退出课程,在课程未完成时也可以输入exit退出");

            Thread.Sleep(1000 * 5);


            Console.WriteLine($"获取到当前浏览器页面有:{Pages.Length}个");
            if (Pages.Length > 0)
            {
                Console.WriteLine("开始匹配所有链接是否为国开课程页面...");
                for (int i = 0; i < Pages.Length; i++)
                {
                    var Page = Pages[i];
                    var Url = Page.Url.ToString();
                    var Name = await Page.GetTitleAsync();
                    if (Regex.IsMatch(Url, @"http://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?id=[0-9]*"))
                    {
                        Console.WriteLine($"匹配成功:({Name}){Url}");
                        CourseList.Add(Name, Page);
                    }
                }
                if (CourseList.Count > 0)
                {
                    Console.WriteLine($"匹配到符合国开课程页面总数:{CourseList.Count}个");
                }
                else
                {
                    Console.WriteLine($"没有匹配到符合国开课程页面,请输入任意键退出...");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine($"当前获取到页面为0,请输入任意键退出...");
                Console.ReadLine();
            }

            Console.WriteLine($"\n开始创建线程...");
            foreach (var item in CourseList)
            {
                var Tas = new System.Threading.Tasks.Task(async () =>
                {
                    var Links = await item.Value.QuerySelectorAllAsync("a[class='aalink']"); //获取第一个页面的所有a标签class为aalink的
                    Console.WriteLine($"[课程:{item.Key}]获取到{Links.Length}个链接");
                    async Task<bool> OpenLink(ElementHandle[] handles)
                    {
                        try
                        {
                            var r = new Random();
                            for (int i = 0; i < Links.Length; i++)
                            {
                                var URL = await Links[i].GetPropertyAsync("href");
                                if (URL.ToString().Replace("JSHandle:", "").Contains(""))
                                {

                                }
                                var Page = await browser.NewPageAsync();
                                await Page.GoToAsync(URL.ToString().Replace("JSHandle:", ""), new NavigationOptions() { });
                                Console.WriteLine($"[{item.Key}]当前正在浏览第{i}个链接:{URL.ToString().Replace("JSHandle:", "")}");
                                var s = r.Next(5, 10);
                                Console.WriteLine($"[{item.Key}]等待{s}秒");
                                await Page.WaitForTimeoutAsync(s * 1000);
                                await Page.CloseAsync(new PageCloseOptions() { RunBeforeUnload = true });
                                Console.WriteLine($"[{item.Key}]关闭页面");
                                Console.WriteLine($"开始下一个\n");
                            }
                        }
                        catch
                        {

                        }
                        return true;
                    }
                    await OpenLink(Links);
                    Console.WriteLine($"[{item.Key}]学习结束 已学习{Links.Length}个链接\n");

                });
                Task.Add(Tas);
            }
            Console.WriteLine($"创建成功！总数{Task.Count}个\n");

            Console.WriteLine($"开始执行！\n");
            Task.ForEach(Course => { Course.Start(); });

            Thread.Sleep(1000 * 5);
            void Exit()
            {
                var msg = Console.ReadLine();
                if (msg == "exit")
                {
                }
                else
                {
                    Console.WriteLine($"请输入exit退出学习...");
                    Exit();
                }
            }

            Exit();
        }

        #endregion

        #region 基本使用方法

        /// <summary>
        /// 检查下载浏览器
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> CheckDownloadBrowser()
        {
            Console.WriteLine("----------------------------------------------------------------------");
            var br = new BrowserFetcher();
            var ExecutablePath = await br.GetRevisionInfoAsync();

            var Downloaded = ExecutablePath.Downloaded;
            Console.WriteLine($"检查当前是否下载浏览器:{(Downloaded == true ? "已下载" : "未下载")}");

            if (!Downloaded)
            {
                Console.WriteLine($"准备开始下载浏览器");
                Console.WriteLine($"平台:{ExecutablePath.Platform}");
                Console.WriteLine($"当前默认选择浏览器:{br.Product}");
                Console.WriteLine($"默认下载浏览器版本:{ExecutablePath.Revision}");

                /*Console.WriteLine($"Local:{ExecutablePath.Local}");
                Console.WriteLine($"文件夹路径:{ExecutablePath.FolderPath}");
                Console.WriteLine($"执行文件路径:{ExecutablePath.ExecutablePath}");
                Console.WriteLine($"手动下载地址:{ExecutablePath.Url}");*/

                Console.WriteLine("--------------------");
                AnsiConsole.MarkupLine("[green]开始[/]下载浏览器");
                br.DownloadProgressChanged += (a, b) => //下载进度条事件
                {
                    if (b.ProgressPercentage >= 100)
                    {
                        AnsiConsole.MarkupLine("下载浏览器[red]完毕[/]");
                    }
                };

                await br.DownloadAsync();
            }
            else
            {
                Console.WriteLine($"平台:{ExecutablePath.Platform}");
                Console.WriteLine($"当前浏览器:{br.Product}");
                Console.WriteLine($"当前浏览器版本:{ExecutablePath.Revision}");
            }

            Console.WriteLine("----------------------------------------------------------------------");
            return true;
        }

        /// <summary>
        /// 网页截图
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="outputFile">输出地址</param>
        /// <returns></returns>
        public async static Task<bool> TakeScreenshots(string url, string outputFile)
        {
            await using (browser)
            {
                await using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url);

                    await page.ScreenshotAsync(outputFile);
                    //await page.ScreenshotBase64Async(outputFile);
                    //await page.ScreenshotDataAsync(outputFile);
                    //await page.ScreenshotStreamAsync(outputFile);
                };
            };
            return true;
        }

        /// <summary>
        /// 网页pdf生成
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="outputFile">输出地址</param>
        /// <returns></returns>
        public async static Task<bool> GeneratePdf(string url, string outputFile)
        {
            await using (browser)
            {
                await using (browser)
                {
                    await using (var page = await browser.NewPageAsync())
                    {
                        await page.GoToAsync(url);

                        await page.PdfAsync(outputFile);


                        /*await page.PdfAsync(outputFile, new PdfOptions //diy pfd设置
                        {
                            Format = PaperFormat.A4,
                            DisplayHeaderFooter = true,
                            MarginOptions = new MarginOptions
                            {
                                Top = "20px",
                                Right = "20px",
                                Bottom = "40px",
                                Left = "20px"
                            },
                            FooterTemplate = "<div id=\"footer-template\" style=\"font-size:10px !important; color:#808080; padding-left:10px\">Footer Text</div>"
                        });*/
                        //await page.PdfDataAsync(outputFile);
                        //await page.PdfStreamAsync(outputFile);
                    };
                };
            }

            return true;
        }

        /// <summary>
        /// 注入html
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> InjectHTML(string url, string html)
        {
            await using (browser)
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url);

                    //await page.SetContentAsync("<div>My Receipt</div>");
                    await page.SetContentAsync(html);
                    var result = await page.GetContentAsync(); //获取网页html
                }
            }

            return true;
        }

        /// <summary>
        /// EvaluateJavascript
        /// </summary>
        /// <returns></returns>
        public async static Task<bool> EvaluateJavascript(string url)
        {
            await using (browser)
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url);

                    var seven = await page.EvaluateExpressionAsync<int>("4 + 3");
                    var html = await page.EvaluateExpressionAsync("document.body.outerHTML");
                    var Text = await page.EvaluateExpressionAsync("document.body.outerText");
                    var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
                    Console.WriteLine(someObject.a);
                }
            }

            return true;
        }

        /// <summary>
        /// 浏览器页面导航
        /// </summary>
        public async static Task<bool> BrowsernPageNavigation(string url)
        {
            await using (browser)
            {
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url);

                    /*Thread.Sleep(1000 * 3);
                    await page.GoBackAsync(new NavigationOptions() //返回上一级导航
                    {
                        *//* Referer= new Dictionary<string, string>(),//http Referer头
                         WaitUntil = WaitUntilNavigation.Load, //当考虑导航成功时，默认为PuffTeeErthApple WaistunLavigal.Load 
                         Timeout = //导航超时时间，默认为30秒，超过0则禁用超时。*//*
                    });

                    Thread.Sleep(1000 * 3);
                    await page.GoForwardAsync(); // Navigate to the next page in history.
                    Thread.Sleep(1000 * 3);*/
                }
            }
            return true;
        }

        #endregion
    }
}
