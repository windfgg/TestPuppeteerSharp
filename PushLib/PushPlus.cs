using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushLib
{
    /// <summary>
    /// PushPlus 推送加
    /// </summary>
    public class PushPlus
    {
        /// <summary>
        /// 
        /// </summary>
        public string token { get; }

        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; } = "http://www.pushplus.plus/send";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        public PushPlus(string token)
        {
            this.token = token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<JObject> SendMsg(string text, PushPlusTemplate template = PushPlusTemplate.html)
        {
            JObject Resutls = null;

            var Client = new HttpClient();
            Client.Timeout = new TimeSpan(0, 0, 5);
            try
            {
                HttpContent content = new StringContent(new PushPlusModel()
                {
                    token = this.token,
                    content = text,
                    template = template.ToString()
                }.ToString());
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var Response = await Client.PostAsync($"{url}", content);
                if (Response != null)
                {
                    Resutls = JObject.Parse(await Response.Content.ReadAsStringAsync());
                    if ((string)Resutls["msg"] != "请求成功")
                        throw new Exception("PushPlus SendMsg Error" + (string)Resutls["msg"]);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Client.Dispose();
            }

            return Resutls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tittle"></param>
        /// <returns></returns>
        public async Task<JObject> SendMsg(string text, string title, PushPlusTemplate template = PushPlusTemplate.html)
        {
            JObject Resutls = null;

            var Client = new HttpClient();
            Client.Timeout = new TimeSpan(0, 0, 5);
            try
            {
                var body = new StringContent(new PushPlusModel()
                {
                    token = this.token,
                    title = title,
                    content = text,
                    template = template.ToString()
                }.ToString());
                HttpContent content = body;
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var Response = await Client.PostAsync($"{url}", content);
                if (Response != null)
                {
                    Resutls = JObject.Parse(await Response.Content.ReadAsStringAsync());
                    if ((string)Resutls["msg"] != "请求成功")
                        throw new Exception("PushPlus SendMsg Error" + (string)Resutls["msg"]);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Client.Dispose();
            }

            return Resutls;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tittle"></param>
        /// <param name="template"></param>
        /// <returns></returns>
      /*  public async Task<JObject> SendChannelMsg(string text, string tittle, PushPlusTemplate template = PushPlusTemplate.html, PushPlusChannel channel= PushPlusChannel.wechat)
        {
            JObject Resutls = null;

            var Client = new HttpClient();
            Client.Timeout = new TimeSpan(0, 0, 5);
            try
            {
                HttpContent content = new StringContent(new PushPlusModel()
                {
                    token = this.token,
                    tittle = tittle,
                    content = text,
                    template = template.ToString()
                }.ToString());
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var Response = await Client.PostAsync($"{url}", content);
                if (Response != null)
                {
                    Resutls = JObject.Parse(await Response.Content.ReadAsStringAsync());
                    if ((string)Resutls["msg"] != "请求成功")
                        throw new Exception("PushPlus SendMsg Error" + (string)Resutls["msg"]);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Client.Dispose();
            }

            return Resutls;
        }*/
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PushPlusTemplate
    {
        /// <summary>
        /// html
        /// </summary>
        html,
        /// <summary>
        /// 纯文本
        /// </summary>
        txt,
        /// <summary>
        /// json
        /// </summary>
        json,
        /// <summary>
        /// markDown
        /// </summary>
        markdown,
        /// <summary>
        /// 阿里云报警模板
        /// </summary>
        cloudMonitor
    }

    /// <summary>
    /// 发送参数模型
    /// </summary>
    public class PushPlusModel
    {
        public string token { get; set; }
        public string title { get; set; } = "";
        public string content { get; set; }
        public string template { get; set; }
        public string channel { get; set; } = "";
        public string webhook { get; set; } = "";

        public override string ToString()
        {
            var json = new JObject();
            json.Add("token", token);
            json.Add("title", title);
            json.Add("content", content);
            json.Add("template", template);
            if (!string.IsNullOrWhiteSpace(channel))
            {
                json.Add("template", template);
                json.Add("webhook", webhook);
            }

            return json.ToString();
        }
    }

    /// <summary>
    /// 发送渠道枚举
    /// </summary>
    public enum PushPlusChannel
    {
        /// <summary>
        /// 微信公众号,默认发送渠道
        /// </summary>
        wechat,

        /// <summary>
        /// 第三方webhook服务；企业微信机器人、钉钉机器人、飞书机器人
        /// </summary>
        webhook,

        /// <summary>
        /// 企业微信应用
        /// </summary>
        cp,

        /// <summary>
        /// 邮件
        /// </summary>
        mail,

        /// <summary>
        /// 短信，未开放使用
        /// </summary>
        //sms
    }
}
