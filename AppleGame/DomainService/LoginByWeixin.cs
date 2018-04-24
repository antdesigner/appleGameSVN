using AntDesigner.GameCityBase.interFace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesigner.GameCityBase;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using GameCitys.DomainService;

namespace AntDesigner.GameCityBase
{
    public class LoginByWeixin : ABIStorehouse, ILoginGame
    {
        public Func<Player, Player> DaddPlayer
        {
            get;
            set;
        }

        public Func<string, Player> DgetPlayerByWeixianName
        {
            get;
            set;
        }
        public static readonly string token = "ant";

        public LoginByWeixin(IStorehouse istoreHose) : base(istoreHose)
        {
        }

        public Player Login(string name,string shareId)
        {
            var player = DgetPlayerByWeixianName(name);
            if (player == null)
            {
                Account account = new Account(name)
                {
                    Balance = 3
                };
                Player newPlayer = new Player(name)
                {
                    Account = account,
                    IntroducerWeixinName = shareId
                };
                player = DaddPlayer(newPlayer);
            }
            _storeHouse.SaveChanges();
            return player;

        }
        public static string GetOpenId(string code)
        {

            string appId = WxPayAPI.WxPayConfig.APPID;
            string secret = WxPayAPI.WxPayConfig.APPSECRET;
            string getOpenIdUrl = "https://api.weixin.qq.com/sns/oauth2/access_token?"
               + "appid=" + appId
                + "&secret=" + secret
                 + "&code=" + code
                 + "&grant_type=authorization_code";
            HttpWebRequest httpWebRequest = (HttpWebRequest)(HttpWebRequest.Create(getOpenIdUrl));
            System.Threading.Tasks.Task<WebResponse> getResponseTask = httpWebRequest.GetResponseAsync();
            getResponseTask.Wait(); 
            using (HttpWebResponse httpWebResponse = getResponseTask.Result as HttpWebResponse)
            { 
            Stream stream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            string responseString = streamReader.ReadToEnd();
            httpWebRequest.Abort();
           LoginByWeixin.Json_access_token json_access_token = JsonConvert.DeserializeObject<LoginByWeixin.Json_access_token>(responseString);
            return json_access_token.Openid;
            }

        }


    public class Json_access_token
    {
        public string Access_token { get; set; }
        public string Expires_in { get; set; }
        public string Refresh_token { get; set; }
        public string Openid { get; set; }
        public string Scope { get; set; }
        public Json_access_token()
        {

        }
    }
    public static bool CheckedSignature(string timestamp, string nonce, string signature)
    {

        string[] arr = { token, timestamp, nonce };
        Array.Sort(arr);
        string arrStr = String.Join("", arr);
        if (ToolsSecret.GetSHA1(arrStr) == signature)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
}
