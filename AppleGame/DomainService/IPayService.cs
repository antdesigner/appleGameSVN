using AntDesigner.GameCityBase;
using AntDesigner.weiXinPay;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace GameCitys.DomainService
{
    public interface IPayService
    {
        string WithdrawCashViewName { get; }//取现页面名称
        string RechargeViewName { get; }//充值页面名称
        
      
        PayOrder CreatPayOrder(Player player, decimal amount);//支付订单
        string GiveRedPackTo(Player player, decimal amount);//取现
        IList<RedPack> GetRedPackgeList(DateTime fromDate, DateTime toDate);//取现明细
        IList<PayOrder> GetPayOrderList(DateTime fromDate, DateTime toDate);//支付订单明细
        PayOrder FindPayOrder(object obj);//找到支付订单
       // string CreateWxJsApiParam(PayOrder payOrder);
        PayOrder CompletePayOrder(object obj);//完成支付订单
    }
}