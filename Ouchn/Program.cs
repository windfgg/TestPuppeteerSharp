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
using Serilog;
using Serilog.Core;

namespace Ouchn;

/// <summary>
/// 
/// </summary>
public static class Program
{
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


                                          --国开自动刷课
                                          --Blog:http://qifengg.xyz/";

            WriteSuccessLine(Info, Color.Green);

            await StartUpBrowser(); //启动浏览器

            await LoginOuchn(); //登录国开

            WriteInfoLine("正在获取课程页面...");
            await MainPage.WaitForTimeoutAsync(1000);
            var Buttons = await MainPage.QuerySelectorAllAsync("button[class='btn bg-primary']");
            var ExistCourseUrl = new List<string>();
            ExistCourseUrl.Add(MainPage.Url);
            for (int i = 0; i < Buttons.Length; i++)
            {
                await Buttons[i].ClickAsync(); //点击学习课程
                await MainPage.WaitForTimeoutAsync(1000 * 2); //等待一秒
                var Pages = await Browser.PagesAsync();
                var CourseUrlIndex = i == 0 ? 1 : 0;
                var CourseUrl = Pages[CourseUrlIndex].Url;

                foreach (var c in Pages)
                {
                    if (!ExistCourseUrl.Contains(c.Url))
                    {
                        CourseUrl = c.Url;
                        ExistCourseUrl.Add(c.Url);
                    }
                }


                LearningCourses[i].CourseUrl = CourseUrl;

                Pages.ToList().ForEach(x =>
               {
                   if (x.Url == LearningCourses[i].CourseUrl)
                       LearningCourses[i].Page = x;
               });
                await MainPage.BringToFrontAsync();
            }
            WriteSuccessLine("获取课程页面完毕！\n");

            var Courses = new List<string>();
            LearningCourses.ForEach((i) =>
            {
                Courses.Add(i.CourseName);
                WriteInfoLine(i.CourseName + $"  课程网址:{(i.CourseUrl)}");
            });
            var CoursesSelector = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
               .Title("请输入需要学习的[green]课程的ID[/]:")
               .NotRequired() // Not required to have a favorite fruit
               .PageSize(10)
               .MoreChoicesText("[grey](使用上下移动来进行选择)[/]")
               .InstructionsText(
                   "[grey](按 [blue]<空格>[/]来进行勾选或取消, " +
                   "按[green]<enter>[/]表示勾选完毕)[/]")
               .AddChoices(Courses));

            WriteSuccessLine($"开始计时...");

            sw.Start();
            foreach (var item in CoursesSelector)
            {
                foreach (var i in LearningCourses)
                {
                    if (i.CourseName == item)
                        await StudyCourse(i);
                }
            }
            sw.Stop();
            WriteSuccessLine($"学习完毕,本次学习耗时:{sw.Elapsed}");


            Console.WriteLine("请按下任意键退出...");
            Console.ReadKey();
            Environment.Exit(-1);
        }

        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            sw.Stop();
            WriteErrorLine("发生错误...");
            WriteSuccessLine($"本次学习耗时:{sw.Elapsed}");
        }
        finally
        {

        }
    }

    public static Logger Log { get; set; } = new LoggerConfiguration()
       .MinimumLevel.Debug()
       .WriteTo.Console()
       .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log-") + ".txt", rollingInterval: RollingInterval.Day)//将日志输出到目标路径，文件的生成方式为每天生成一个文件
       .CreateLogger();

    public static List<LearningCourse> LearningCourses { get; set; }
    public static Browser Browser { get; set; }
    public static Page MainPage { get; set; }
    public static Configure Configure { get; set; }
    public static string Cookies { get; set; } = "";

    #region 控制台打印
    /// <summary>
    /// 
    /// </summary>
    /// <param name="length"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    static string[] ToArrayString(this int length, int startIndex = 0)
    {
        var ArrayString = new List<string>();
        for (int i = startIndex; i < length; i++)
        {
            ArrayString.Add(i.ToString());
        }
        return ArrayString.ToArray();
    }
    static void WriteColorLine(string str, ConsoleColor color)
    {
        ConsoleColor currentForeColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(str);
        Console.ForegroundColor = currentForeColor;
    }
    /// <summary>
    /// 打印错误信息
    /// </summary>
    /// <param name="str">待打印的字符串</param>
    /// <param name="color">想要打印的颜色</param>
    public static void WriteErrorLine(this string str, ConsoleColor color = ConsoleColor.Red)
    {
        WriteColorLine(str, color);
    }

    /// <summary>
    /// 打印警告信息
    /// </summary>
    /// <param name="str">待打印的字符串</param>
    /// <param name="color">想要打印的颜色</param>
    public static void WriteWarningLine(this string str, ConsoleColor color = ConsoleColor.Yellow)
    {
        WriteColorLine(str, color);
    }
    /// <summary>
    /// 打印正常信息
    /// </summary>
    /// <param name="str">待打印的字符串</param>
    /// <param name="color">想要打印的颜色</param>
    public static void WriteInfoLine(this string str, ConsoleColor color = ConsoleColor.White)
    {
        WriteColorLine(str, color);
    }

    /// <summary>
    /// 打印成功的信息
    /// </summary>
    /// <param name="str">待打印的字符串</param>
    /// <param name="color">想要打印的颜色</param>
    public static void WriteSuccessLine(this string str, ConsoleColor color = ConsoleColor.Green)
    {
        WriteColorLine(str, color);
    }

    /// <summary>
    /// 退出
    /// </summary>
    /// <param name="msg"></param>
    public static void Exit(string msg = "")
    {
        WriteErrorLine($"{msg}\n请输入任意键退出...");
        Console.ReadLine();
        Environment.Exit(-1);
    }
    #endregion

    #region  国开操作
    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    public static async Task<string> GetUserInfo(string acess_token)
    {
        var Resutls = new StringBuilder();

        var client = new RestClient("http://passport.ouchn.cn/connect/userinfo");
        client.Timeout = 5000;
        var request = new RestRequest(Method.GET);
        request.AddHeader("Authorization", $"Bearer {acess_token}");
        var response = await client.ExecuteAsync(request);
        var UserInfo = JObject.Parse(response.Content.ToString());
        Resutls.AppendLine("昵称:" + UserInfo["nickname"].ToString());

        return Resutls.ToString();
    }

    /// <summary>
    /// 获取学习档案
    /// </summary>
    /// <param name="acess_token"></param>
    /// <returns></returns>
    public static async Task<string> GetLearningArchives(string acess_token)
    {
        var Resutls = new StringBuilder();

        var client = new RestClient("http://117.78.41.66:9005/api/MyCourse/LearningArchives");
        client.Timeout = 5000;
        var request = new RestRequest(Method.GET);
        request.AddHeader("Authorization", $"Bearer {acess_token}");
        var response = await client.ExecuteAsync(request);
        var UserInfo = JObject.Parse(response.Content.ToString());
        Resutls.AppendLine("专业:" + UserInfo["Data"]["SpecialtyName"].ToString() + $"({UserInfo["Data"]["SpecialtyLevelName"].ToString()})");
        Resutls.AppendLine("当前学期:" + UserInfo["Data"]["SemesterName"].ToString());
        Resutls.AppendLine("机构名称:" + UserInfo["Data"]["OrganizationName"].ToString());

        return Resutls.ToString();
    }

    /// <summary>
    /// 国开官网获取课程信息
    /// </summary>
    /// <param name="acess_token"></param>
    /// <returns></returns>
    public static async Task GetCoursesSemestersInfo(string acess_token)
    {
        var Resutls = new StringBuilder();
        var client = new RestClient("http://117.78.41.66:9005/api/MyCourse/CoursesSemestersInfo");
        client.Timeout = 5000;
        var request = new RestRequest(Method.GET);
        request.AddHeader("Authorization", $"Bearer {acess_token}");
        var response = await client.ExecuteAsync(request);
        var UserInfo = JObject.Parse(response.Content.ToString());
        LearningCourses = JsonConvert.DeserializeObject<List<LearningCourse>>(UserInfo["Data"]["LearningCourses"].ToString());
        if (LearningCourses.Count > 0)
        {
            // Create a table
            var table = new Table();

            // Add some columns
            table.AddColumn("Id");
            table.AddColumn("课程");
            table.AddColumn("学习进度");
            table.AddColumn("形考成绩");
            table.AddColumn("学分");
            table.AddColumn("未完成的作业和测验");

            for (int i = 0; i < LearningCourses.Count; i++)
            {
                var item = LearningCourses[i];
                var CourseName = $"{item.CourseName}({item.CourseNatureName})";
                var ExamScore = $"{item.ExamScore}";
                var Credit = item.Credit.ToString();
                var UnCompletedTestAssignCount = item.UnCompletedTestAssignCount;
                var Chu = Math.Round((double)(item.CompletedFormativeTestCount / item.ActivityCount), 2);
                var Learning = Chu * 100;
                table.AddRow(i.ToString(), CourseName, Learning.ToString() + "%", ExamScore, Credit, UnCompletedTestAssignCount);
            }

            Console.WriteLine($"  班级名称:{LearningCourses[0].CourseClassName}");
            AnsiConsole.Write(table);
        }

    }

    /// <summary>
    /// 国开分部获取课程信息
    /// </summary>
    /// <returns></returns>
    public static async Task GetBranchCoursesSemestersInfo()
    {
        var client = new RestClient("http://guangzhou.ouchn.cn/lib/ajax/service.php?sesskey=h6m8G6iclS&info=core_course_get_enrolled_courses_by_timeline_classification");
        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Cookie", Cookies);
        var body = @"[
" + "\n" +
        @"    {
" + "\n" +
        @"        ""index"": 0,
" + "\n" +
        @"        ""methodname"": ""core_course_get_enrolled_courses_by_timeline_classification"",
" + "\n" +
        @"        ""args"": {
" + "\n" +
        @"            ""offset"": 0,
" + "\n" +
        @"            ""limit"": 0,
" + "\n" +
        @"            ""classification"": ""all"",
" + "\n" +
    @"            ""sort"": ""fullname"",
" + "\n" +
        @"            ""customfieldname"": """",
" + "\n" +
        @"            ""customfieldvalue"": """"
" + "\n" +
        @"        }
" + "\n" +
        @"    }
" + "\n" +
        @"]";
        request.AddParameter("application/json", body, ParameterType.RequestBody);
        var response = await client.ExecuteAsync(request);
        Console.WriteLine(response.Content);
    }

    /// <summary>
    /// 获取国开分部官网地址 
    /// </summary>
    /// <param name="acess_token"></param>
    /// <returns></returns>
    public static async Task<string> GetOuchnBranchUrl(Page page, string acess_token)
    {
        var Url = "";

        var client = new RestClient("http://117.78.41.66:9005/api/MyCourse/CoursesSemestersInfo");
        client.Timeout = 5000;
        var request = new RestRequest(Method.GET);
        request.AddHeader("Authorization", $"Bearer {acess_token}");
        var response = await client.ExecuteAsync(request);
        var UserInfo = JObject.Parse(response.Content.ToString());
        var LearningCourses = JsonConvert.DeserializeObject<List<LearningCourse>>(UserInfo["Data"]["LearningCourses"].ToString());

        var Buttons = await page.QuerySelectorAllAsync("button[class='btn bg-primary']");
        await Buttons[0].ClickAsync();

        await page.WaitForTimeoutAsync(1000);
        var Page = await Browser.NewPageAsync();
        await Page.GoToAsync(Url);
        var ck = await Page.GetCookiesAsync();
        foreach (var item in ck)
        {
            Cookies += $"{item.Name}={item.Value};";
        }

        return Url;
    }

    /// <summary>
    /// 学习课程
    /// </summary>
    public static async Task StudyCourse(LearningCourse course)
    {
        await course.Page.BringToFrontAsync();
        await course.Page.WaitForTimeoutAsync(1000);
        Log.Information($"切换到课程:{course.CourseName}页面");
        var Links = await course.Page.QuerySelectorAllAsync("a[class='aalink']"); //获取第一个页面的所有a标签class为aalink的
        Log.Information($"[课程:{course.CourseName}]获取到{Links.Length}个链接");
        async Task OpenLink()
        {
            var r = new Random();
            for (int i = 0; i < Links.Length; i++)
            {
                var URL = await Links[i].GetPropertyAsync("href");
                var Page = await Browser.NewPageAsync();
                Page.Error += async (a, b) =>
                {
                    await Page.CloseAsync();
                };
                Page.DefaultTimeout = 60000;
                var Href = URL.ToString().Replace("JSHandle:", "");
                if (Href == "about:blank")
                {
                    Log.Error("Herf为空页面，跳过本链接");
                }
                else
                {
                    await Page.GoToAsync(Href);
                    Log.Information($"[{course.CourseName}]当前正在浏览第{i}个链接:{Href}");
                    //Log.Debug(await Page.GetContentAsync());
                    Log.Debug($"网页标题:" + await Page.GetTitleAsync());
                    var s = r.Next(Configure.MinSeconds, Configure.MaxSeconds);
                    Log.Information($"[{course.CourseName}]等待{s}秒");
                    await Page.WaitForTimeoutAsync(s * 1000);
                    await Page.CloseAsync();
                    Log.Information($"[{course.CourseName}]关闭页面");
                }

                Log.Information($"开始下一个课程\n");
            }
        }
        await OpenLink();
        Log.Information($"[{course.CourseName}]学习结束 已学习{Links.Length}个链接\n");
    }
    #endregion

    #region 配置文件读取
    /// <summary>
    /// 
    /// </summary>
    public static Task ReadConfigure()
    {
        var ConfigurePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configure.yml");
        if (File.Exists(ConfigurePath))
        {
            try
            {
                var deserializer = new DeserializerBuilder().Build();
                Configure = deserializer.Deserialize<Configure>(File.ReadAllText(ConfigurePath));
                WriteSuccessLine($"读取配置文件成功...");
            }
            catch (Exception ex)
            {
                Exit($"配置文件错误{ex},请按照教程下载并修改配置文件");
            }

        }
        else Exit($"配置文件不存在,请按照教程下载并修改配置文件");

        return Task.CompletedTask;
    }
    #endregion

    #region 浏览器操作
    /// <summary>
    /// 启动浏览器
    /// </summary>
    /// <returns></returns>
    public static async Task StartUpBrowser()
    {
        await CheckDownloadBrowser();

        var extra = new PuppeteerExtra()
            .Use(new StealthPlugin());

        await ReadConfigure();
        try
        {
            Browser = await extra.LaunchAsync(new LaunchOptions()
            {
                Headless = !Configure.Debug,
                IgnoredDefaultArgs = new string[] { "--incognito" }
            });
        }
        catch
        {
            WriteErrorLine($"连接到浏览器失败,请按照教程再启动一遍浏览器试试...");
            Environment.Exit(-1);
        }
        WriteSuccessLine($"启动浏览器成功!");
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

            WriteSuccessLine($"正在下载浏览器,因浏览器较大请耐心等待....");

            br.DownloadProgressChanged += (a, b) => //下载进度条事件
            {
                if (b.ProgressPercentage >= 100)
                {
                    WriteSuccessLine("下载浏览器完毕");
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

    /// <summary>
    /// 登录国开
    /// </summary>
    /// <returns></returns>
    public static async Task LoginOuchn()
    {
        MainPage = await Browser.NewPageAsync();

        var UserNo = Configure.UserNo;
        var Password = Configure.Password;

        await MainPage.GoToAsync("http://student.ouchn.cn/");
        await MainPage.WaitForNavigationAsync();

        await MainPage.TypeAsync("#username", UserNo);
        await MainPage.TypeAsync("#password", Password);
        //Page.EvaluateExpressionAsync("")
        var LoginButton = await MainPage.WaitForXPathAsync("/html/body/div/div/div/form/div/div/div[4]/button");
        await LoginButton.ClickAsync();

        await MainPage.WaitForNavigationAsync();
        var ErrorMsg = await MainPage.QuerySelectorAllAsync("body > div > div > div.alert.alert-danger > div > ul > li");
        if (ErrorMsg.Length > 0)
        {
            var Msg = await ErrorMsg[0].GetPropertyAsync("innerHTML");
            WriteErrorLine($"登录失败:{Msg.ToString().Replace("JSHandle:", "")}");

            Exit("请将用户名和密码修改正确后重新启动(错误次数过多会导致用户锁定)");
        }
        else
        {
            WriteSuccessLine($"登录成功\n");
            var access_token = MainPage.Url.Split('&')[1].Replace("access_token=", "");
            AnsiConsole.Write(new Panel(await GetUserInfo(access_token) + await GetLearningArchives(access_token)));
            await GetCoursesSemestersInfo(access_token);

            var Pages = await Browser.PagesAsync();
            Pages.ToList().ForEach(page =>
            {
                if (page.Url == "about:blank")
                    page.CloseAsync();
            });
            //var BranchUrl = await GetOuchnBranchUrl(Page, access_token);
            //await GetBranchCoursesSemestersInfo();
            //Console.WriteLine(Page.Url);
            //await Page.GoToAsync(BranchUrl);
        }
    }
    #endregion
}
