
using System.Text;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WxPayAPI
{
    /// <summary>
    /// 回调处理基类
    /// 主要负责接收微信支付后台发送过来的数据，对数据进行签名验证
    /// 子类在此类基础上进行派生并重写自己的回调处理过程
    /// </summary>
    public class Notify
    {
        public HttpContext httpContext {get;set;}
        public Notify(HttpContext httpContext_)
        {
            httpContext = httpContext_;
         
        
        }

        /// <summary>
        /// 接收从微信支付后台发送过来的数据并验证签名
        /// </summary>
        /// <returns>微信支付后台返回的数据</returns>
        public WxPayData GetNotifyData()
        {
            //转换数据格式并验证签名
            StreamReader streamReader = new StreamReader(httpContext.Request.Body, Encoding.UTF8);
            string resultStr = streamReader.ReadToEnd();
            WxPayData data = new WxPayData();
            try
            {
                data.FromXml(resultStr);
            }
            catch(WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                httpContext.Response.WriteAsync(res.ToXml());
            }
            return data;
        }

        //派生类需要重写这个方法，进行不同的回调处理
        public virtual WxPayData ProcessNotify()
        {
            return null;
        }
    }
}