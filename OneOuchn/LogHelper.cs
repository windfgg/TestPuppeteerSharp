using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneOuchn
{
    /// <summary>
    /// 日志帮助
    /// </summary>
    public static class LogHelper
    {
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
        public static void WriteColorLine(string str, ConsoleColor color)
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
    }
}
