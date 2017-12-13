using AntDesigner.AppleGame;
using AntDesigner.GameCityBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WxPayAPI;

namespace WxPayAPI
{
    /**
    * 	配置账号信息
    */
    public class WxPayConfig {

        //=======【上报信息配置】===================================
        /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        */
        public const int REPORT_LEVENL = 0;

        //=======【日志级别】===================================
        /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
        */
        public const int LOG_LEVENL = 0;
        //=======【基本信息设置】=====================================
        /* 微信公众号信息配置
        * APPID：绑定支付的APPID（必须配置）
        * MCHID：商户号（必须配置）
        * KEY：商户支付密钥，参考开户邮件设置（必须配置）
        * APPSECRET：公众帐号secert（仅JSAPI支付的时候需要配置）
        */
        //public const string APPID = "";
        //public const string MCHID = "";
        //public const string KEY = "";
        //public const string APPSECRET = "";

        ////=======【支付结果通知url】===================================== 
        ///* 支付结果通知回调url，用于商户接收支付结果
        //*/

        //public const string NOTIFY_URL = "";

        ////=======【商户系统后台机器IP】===================================== 
        ///* 此参数可手动配置也可在程序中自动获取
        //*/
        //public const string IP = "";
        ////=======【证书路径设置】===================================== 
        ///* 证书路径,注意应该填写绝对路径（仅退款、撤销订单时需要）
        // * 
        //*/
        //public const string SSLCERT_PATH = "";
        //public const string SSLCERT_PASSWORD = "";
        //public const string certName = "";

        public static string APPID;
        public static string MCHID;
        public static string KEY;
        public static string APPSECRET;
        public static string SiteName;
        public static string SSLCERT_PATH;
        public static string SSLCERT_PASSWORD;
        public static string NOTIFY_URL;
        public static string IP;
        public static string certName;
        public static string CheckK;
        public static string SiteNameNopre;
        private static DateTime Access_tokenSaveTime { get; set; }
        private static MyAccess_token access_token_;
        public static MyAccess_token _access_token {

            get {
                if (access_token_ != null) {
                    TimeSpan timSpan = DateTime.Now - Access_tokenSaveTime;
                    if (timSpan.Seconds < 7000) {
                        return access_token_;
                    }
                }
                string url = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + APPID + "&secret=" + APPSECRET;
                access_token_ = GetWeixinReturnObject<MyAccess_token>(url);
                Access_tokenSaveTime = DateTime.Now;
                return access_token_;
            }
        }
        private static DateTime Jsapi_ticketSaveTime { get; set; }
        private static Jsapi_ticket jsapi_ticket_;
        public static Jsapi_ticket _jsapi_ticket {

            get {
                if (jsapi_ticket_ != null) {
                    TimeSpan timSpan = DateTime.Now - Jsapi_ticketSaveTime;
                    if (timSpan.Seconds < 7000) {
                        return jsapi_ticket_;

                    }

                }
                string url = "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token=" + _access_token.Access_token + "&type=jsapi";
                jsapi_ticket_ = GetWeixinReturnObject<Jsapi_ticket>(url);
                Jsapi_ticketSaveTime = DateTime.Now;
                return jsapi_ticket_;
            }
        }
        public static WeixinIps WeixinIpsList {get;set;}
        private static T GetWeixinReturnObject<T>(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)(HttpWebRequest.Create(url));
            System.Threading.Tasks.Task<WebResponse> getResponseTask = httpWebRequest.GetResponseAsync();
            getResponseTask.Wait();
            using (HttpWebResponse httpWebResponse = getResponseTask.Result as HttpWebResponse)
            {
                Stream stream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                string responseString = streamReader.ReadToEnd();
                httpWebRequest.Abort();
                T objectFromJsonstr = JsonConvert.DeserializeObject<T>(responseString);
                return objectFromJsonstr;
            }
        }

        public static string GetSignature(string url, 
            out string noncestr, out string timestamp)
        {
            noncestr = WxPayApi.GenerateNonceStr();
            string jsapi_ticket = _jsapi_ticket.Ticket;
            timestamp = WxPayApi.GenerateTimeStamp();
            Dictionary<string, string> dictionary = new Dictionary<string, string>
            {
                { "noncestr", noncestr },
                { "jsapi_ticket", jsapi_ticket },
                { "timestamp", timestamp },
                { "url", url }
            };
            Dictionary<string, string> dictionaryAfterSort = dictionary.OrderBy(p => p.Key)
                                                            .ToDictionary(o => o.Key, p => p.Value);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in dictionaryAfterSort)
            {
                stringBuilder.Append(item.Key);
                stringBuilder.Append("=");
                stringBuilder.Append(item.Value);
                stringBuilder.Append("&");
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
             string  signature = ToolsSecret.GetSHA1(stringBuilder.ToString());
            return signature;
        }
        public class MyAccess_token
        {

            public  string Access_token
            {
                get; set;
            }
            public string Expires_in
            {
                get; set;
            }
        }

        public class Jsapi_ticket
        {
            public string Errcode { get; set; }
            public string Errmsg { get; set; }
            public  string Ticket { get; set; }
            public string Expires_in { get; set; }
        }
       public  static WeixinIps GetWeixinIps() {
            if (WeixinIpsList  is null) {
                string url_token = "https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token=" + WxPayConfig._access_token.Access_token;
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url_token);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponseAsync().Result;
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string content = reader.ReadToEnd();
                WeixinIpsList = JsonConvert.DeserializeObject<WeixinIps>(content);
                reader.Dispose();
                myRequest.Abort();
            }
                return WeixinIpsList;
        }
        public class WeixinIps {
            public string[] ip_list { get; set; }
        }
    }

    }



