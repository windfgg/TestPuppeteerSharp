using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneOuchn
{
    public static class ExtentionMethod
    {
        /// <summary>
        /// 将对象序列化成Json字符串
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <param name="indent">是否缩进</param>
        /// <returns></returns>
        public static string ToJson(this object obj, bool indent = true)
        {
            return JsonConvert.SerializeObject(obj, indent
                ? Formatting.Indented
                : Formatting.None);
        }

        /// <summary>
        /// 将Json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonStr">Json字符串</param>
        /// <returns></returns>
        public static T JsonTo<T>(this string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        /// <summary>
        /// Process打开图片
        /// </summary>
        /// <param name="file_name"></param>
        /// <returns></returns>
        public static async Task<Process> OpenImage(string file_name)
        {
            Process process = new Process();
            process.StartInfo.FileName = file_name;
            process.StartInfo.Arguments = "rundll32.exe C://WINDOWS//system32//shimgvw.dll";
            process.StartInfo.UseShellExecute = true;
            process.Start();
            return process;
        }

        /// <summary>
        /// Process打开网址
        /// </summary>
        /// <param name="file_name"></param>
        /// <returns></returns>
        public static void OpenUrl(string url)
        {
            new Task(() =>
            {
                Process.Start("IExplore.exe", url);
            }).Start();
        }
    }
}
