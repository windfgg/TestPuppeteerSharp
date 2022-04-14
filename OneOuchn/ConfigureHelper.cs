using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using OneOuchn;

namespace OneOuchn
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigureHelper
    {
        public ConfigureHelper()
        {
            ReadConfigure();
        }

        public Configure Configure { get; set; }

        #region 配置文件读取
        /// <summary>
        /// 
        /// </summary>
        public Task ReadConfigure()
        {
            var ConfigurePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configure.yml");
            if (File.Exists(ConfigurePath))
            {
                try
                {
                    var Text = File.ReadAllText(ConfigurePath);
                    var deserializer = new DeserializerBuilder().Build().Deserialize<Configure>(Text);
                    Configure = deserializer;
                    //LogHelper.WriteSuccessLine($"读取配置文件成功...");
                }
                catch (Exception ex)
                {
                    LogHelper.Exit($"配置文件错误{ex},请按照教程下载并修改配置文件");
                }

            }
            else LogHelper.Exit($"配置文件不存在,请按照教程下载并修改配置文件");

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void SetCookie(string value)
        {
            Configure.Cookie = value;
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configure.yml"), new SerializerBuilder().Build().Serialize(Configure));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetCookieLogin(bool value)
        {
            Configure.CookieLogin = value;
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configure.yml"), new SerializerBuilder().Build().Serialize(Configure));
        }
        #endregion
    }
}
