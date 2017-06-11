using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WxPayAPI;
using AntDesigner.weiXinPay;
using AntDesigner.GameCityBase;
using AntDesigner.GameCityBase.EF;
using AntDesigner.GameCityBase.interFace;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameCitys.EF;
using Microsoft.AspNetCore.Http;

namespace GameCitys.DomainService
{
    public abstract  class PayServiceBase : ABIStorehouse, IPayService
    {

        /// <summary>
        /// 发红包前端页面名称
        /// </summary>
        public string WithdrawCashViewName { get;protected  set; }
        /// <summary>
        /// 充值前端页面名称
        /// </summary>
        public string RechargeViewName { get; protected set; }
        public PayServiceBase([FromServices]IStorehouse istoreHose) :base(istoreHose)
        {
           
        }
        /// <summary>
        /// 创建支付订单
        /// </summary>
        /// <param name="player">玩家</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        public virtual PayOrder CreatPayOrder(Player player, decimal amount)
        {

            PayOrder payOrder = new PayOrder(player.WeixinName, amount);
       
            _storeHouse.AddEntity<PayOrder>(payOrder);
            _storeHouse.SaveChanges();
            return payOrder;
        }
 
        /// <summary>
        /// 调用外部红包发送接口
        /// </summary>
        /// <param name="redPack">红包</param>
        /// <param name="re_openid_"></param>
        /// <param name="amount_"></param>
        /// <param name="wishing_"></param>
        /// <param name="send_name_"></param>
        /// <param name="act_name_"></param>
        /// <param name="remark_"></param>
        /// <returns></returns>
        /// <summary>
        /// 一段时间发出的红包
        /// </summary>
        /// <param name="fromDate">开始日期</param>
        /// <param name="toDate">结束日期</param>
        /// <returns></returns>
        public virtual IList<RedPack> GetRedPackgeList(DateTime fromDate, DateTime toDate)
        {
            return _storeHouse.GetRedPackgerList(fromDate, toDate);
        }
        /// <summary>
        /// 一段时间的支付订单
        /// </summary>
        /// <param name="fromDate">开始日期</param>
        /// <param name="toDate">结束日期</param>
        /// <returns></returns>
        public  virtual IList<PayOrder> GetPayOrderList(DateTime fromDate, DateTime toDate)
        {
            return _storeHouse.GetPayOrderList(fromDate,toDate);
        }
        /// <summary>
        /// 一段时间的红包奖金记录
        /// </summary>
        /// <param name="fromDate">开始日期</param>
        /// <param name="toDate">结束日期</param>
        /// <returns></returns>
        public virtual  IList<RedPack> RechargeOrders(DateTime fromDate, DateTime toDate)
        {
            return _storeHouse.GetRedPackgerList(fromDate, toDate);
        }

        public abstract PayOrder FindPayOrder(object obj);
        public abstract PayOrder CompletePayOrder(object obj);
        public abstract string GiveRedPackTo(Player player, decimal amount);
    }
}

