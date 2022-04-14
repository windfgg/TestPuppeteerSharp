using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneOuchn
{
    /// <summary>
    /// Http请求类
    /// </summary>
    public class HttpClientHelper
    {
        public HttpClient client = null;

        public HttpClientHelper()
        {
            client = new HttpClient();
        }

        /// <summary>
        /// Post Aaync
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public async Task<string> PostAsync(string url, string strJson)//post异步请求方法
        {
            try
            {
                HttpContent content = new StringContent(strJson);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                //由HttpClient发出异步Post请求
                HttpResponseMessage res = await client.PostAsync(url, content);
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string str = res.Content.ReadAsStringAsync().Result;
                    return str;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public async Task<JObject> PostAsyncJObject(string url, string strJson)
        {
            try
            {
                HttpContent content = new StringContent(strJson);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage res = await client.PostAsync(url, content);
                string str = await res.Content.ReadAsStringAsync();
                return JObject.Parse(str);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> DeleteAsync(string url)
        {
            try
            {
                HttpResponseMessage res = await client.DeleteAsync(url);
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string str = res.Content.ReadAsStringAsync().Result;
                    return str;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string Url)
        {
            try
            {
                var responseString = await client.GetStringAsync(Url);
                return responseString;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<JObject> GetJObjectAsync(string Url)
        {
            try
            {
                var responseString = await client.GetStringAsync(Url);
                return JObject.Parse(responseString);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
