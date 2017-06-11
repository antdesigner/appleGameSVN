using System;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


namespace WxPayAPI
{
    /// <summary>
    /// http连接基础类，负责底层的http通信
    /// </summary>
    public class HttpService
    {
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }
       public static string Post(string xml, string url, bool isUseCert, int timeout)
        {
          //  System.GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            string result = "";//返回结果
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
            Stream reqStream = null;
            try
            {
                if (!isUseCert)
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContinueTimeout = timeout * 1000;
                    request.ContentType = "text/xml";
                    reqStream = request.GetRequestStreamAsync().Result;
                    //往服务器写入数据
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Dispose();
                    //获取服务端返回
                    response = (HttpWebResponse)request.GetResponseAsync().Result;
                    //获取服务端返回数据
                    StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    result = streamReader.ReadToEnd().Trim();
                    streamReader.Dispose();
                    ////是否使用证书
                }
                else { 
                    string path = Directory.GetCurrentDirectory();
                    X509Store store = new X509Store("My", StoreLocation.LocalMachine);
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    System.Security.Cryptography.X509Certificates.X509Certificate2 cert =
                    store.Certificates.Find(X509FindType.FindBySubjectName, WxPayConfig.certName, false)[0];
                    var handler = new HttpClientHandler();
                    handler.ClientCertificates.Add(cert);
                    using (HttpClient client = new HttpClient(handler))
                    {
                        StringContent stringContent = new StringContent(xml, Encoding.UTF8, "text/xml");
                        HttpResponseMessage httpResponseMessage = client.PostAsync(url, stringContent).Result;
                        reqStream = httpResponseMessage.Content.ReadAsStreamAsync().Result;
                        StreamReader streamReader = new StreamReader(reqStream, Encoding.UTF8);
                        result = streamReader.ReadToEnd().Trim();
                        streamReader.Dispose();
                    }
                }
            }

            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Dispose();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return result;
        }

    }
}