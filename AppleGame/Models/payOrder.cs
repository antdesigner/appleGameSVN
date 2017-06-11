
using AntDesigner.GameCityBase.interFace;
using System;

using System.ComponentModel.DataAnnotations.Schema;

using WxPayAPI;

namespace AntDesigner.weiXinPay
{
    public class PayOrder
    {
        public int Id { get; set; }
       public string WeixinName{ get; set; }
        public decimal Amount { get; set; }
        public bool Success { get; set; }
        public string Out_trade_no { get; set; }
        public string ClientIp { get; set; }
        public string Prepay_id { get; set; }
        public string Transaction_id { get; set; }

        public PayOrder()
        { }
        public PayOrder(string weixinName_,decimal amount_)
        {
            Success = false;
            WeixinName = weixinName_;
            Amount = amount_;
        }
        public string CreateWxJsApiParam()
        {
            JsApiPay jsApiPay = new JsApiPay();
            WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(this);
            Prepay_id = (string)unifiedOrderResult.GetValue("prepay_id");
            string wxJsApiParam = jsApiPay.GetJsApiParameters();
            return wxJsApiParam;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreateTime
        {
            get; set;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModifyTime
        { get; set; }
    }
}
