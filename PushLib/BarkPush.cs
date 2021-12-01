using Newtonsoft.Json.Linq;

namespace PushLib
{
    /// <summary>
    /// IOS Bark
    /// </summary>
    public class BarkPush
    {
        /// <summary>
        /// 
        /// </summary>
        public string barkServerAddres { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="barkServerAddres"></param>
        public BarkPush(string barkServerAddres)
        {
            this.barkServerAddres = barkServerAddres;
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="text">内容</param>
        /// <returns></returns>
        public async Task<JObject> SendTextMsg(string text, Dictionary<BarkParameter, string> Params = null)
        {
            JObject Resutls = null;

            var Client = new HttpClient();
            Client.Timeout = new TimeSpan(0, 0, 5);
            try
            {
                var url = $"{barkServerAddres}{text}?";
                if (Params != null && Params.Count > 0)
                {
                    foreach (var item in Params)
                    {
                        url += $"{item.Key}={item.Value}&";
                    }
                }

                var Response = await Client.GetAsync(url);
                if (Response != null)
                {
                    if (Response.StatusCode != System.Net.HttpStatusCode.OK) 
                        throw new Exception("BarkPush SendTextMsg Error" + await Response.Content.ReadAsStringAsync());
                    Resutls = JObject.Parse(await Response.Content.ReadAsStringAsync());
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
        /// 发送文本消息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="text">内容</param>
        /// <param name="tittle">标题</param>
        /// <returns></returns>
        public async Task<JObject> SendTextMsg(string text, string tittle, Dictionary<BarkParameter, string> Params = null)
        {
            JObject Resutls = null;

            var Client = new HttpClient();
            Client.Timeout = new TimeSpan(0, 0, 5);
            try
            {
                var url = $"{barkServerAddres}{tittle}/{text}?";
                if (Params != null && Params.Count > 0)
                {
                    foreach (var item in Params)
                    {
                        url += $"{item.Key}={item.Value}&";
                    }
                }
                var Response = await Client.GetAsync(url);

                if (Response != null)
                {
                    if (Response.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new Exception("BarkPush SendTextMsg Error" + await Response.Content.ReadAsStringAsync());
                    Resutls = JObject.Parse(await Response.Content.ReadAsStringAsync());
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
    }

    /// <summary>
    /// Bark可选参数
    /// </summary>
    public enum BarkParameter
    {
        /// <summary>
        /// 铃声 BarkSound
        /// </summary>
        sound,

        /// <summary>
        /// 是否保存消息 值为1自动保存
        /// </summary>
        isArchive,

        /// <summary>
        /// 自定义推送图标（需iOS15或以上）icon地址
        /// </summary>
        icon,

        /// <summary>
        /// 推送消息分组
        /// </summary>
        group,

        /// <summary>
        /// 跳转地址
        /// </summary>
        url,

        /// <summary>
        /// 复制内容
        /// </summary>
        copy,

        /// <summary>
        /// 角标
        /// </summary>
        badge,
    }

    /// <summary>
    /// 推送铃声
    /// </summary>
    public enum BarkSound
    {
        alarm,
        anticipate,
        bell,
        birdsong,
        bloom,
        calypso,
        chime,
        choo,
        descent,
        electronic,
        fanfare,
        glass,
        healthnotification,
        horn,
        ladder,
        mailsent,
        minuet,
        multiwayinvitation,
        newmail,
        newsflash,
        noir,
        paymentsuccess,
        shake,
        sherwoodforest,
        silence,
        spell,
        suspense,
        telegraph,
        tiptoes,
        typewriters,
        update,
    }
}