using AntDesigner.GameCityBase;
using AntDesigner.GameCityBase.Controllers;
using AntDesigner.GameCityBase.interFace;
using AntDesigner.weiXinPay;
using GameCitys.DomainService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using WxPayAPI;

namespace AntDesigner.Controllers
{
    public class WeixinPayController : MyController
    {
 
        public WeixinPayController( IHttpContextAccessor httpContextAccessor, IPlayerService playerService) : base( httpContextAccessor, playerService)
        {
           
        }
        [AllowAnonymous]
        public IActionResult GetPayResult([FromServices]IPayServiceWeixin payServiceWeixin)
        {
            PayOrder payOrder = payServiceWeixin.CompletePayOrderAndReback(httpContextAccessor.HttpContext, out string successStr);
            if (payOrder.Success==true)
            {
                return Content(successStr);
            }
            else
            {
                return null;
            }
           
        }
    }
}
