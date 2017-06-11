using AntDesigner.weiXinPay;
using Microsoft.AspNetCore.Http;

namespace GameCitys.DomainService
{
    public interface IPayServiceWeixin:IPayService
    {
        PayOrder CompletePayOrderAndReback(HttpContext httpContext, out string successStr);
        string CreateWxJsApiParam(PayOrder payOrder);
    }
}