﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WxPayAPI;

namespace AntDesigner.weiXinPay
{
    public class WeixinPayManager
    {

        public Func<WxPayData, PayOrder> DcompletePayOrder { get; set; }
        public PayOrder CompletePayOrderAndReback(HttpContext httpContext ,out string successStr)
        {

            ResultNotify resultNotify = new ResultNotify(httpContext);
            WxPayData WxPayData = resultNotify.ProcessNotify();
            PayOrder payOrder = null;
            successStr = "";
            if (WxPayData != null)
            {
                payOrder = DcompletePayOrder(WxPayData);
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
               // httpContext.Response.WriteAsync();
                successStr = res.ToXml();
            }
            return payOrder;
        }
    }
}
