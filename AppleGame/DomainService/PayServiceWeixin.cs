using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesigner.GameCityBase.interFace;
using GameCitys.EF;
using Microsoft.AspNetCore.Mvc;
using AntDesigner.weiXinPay;
using WxPayAPI;
using AntDesigner.GameCityBase;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCitys.DomainService
{

    public class PayServiceWeixin : PayServiceBase, IPayServiceWeixin
    {
        IPlayerService _playerService;
        IStoreHoseForWeixin _storeHoseForWeixin;
        private readonly ILogger _logger;
        public PayServiceWeixin([FromServices]IStorehouse istoreHose
        , [FromServices]IPlayerService playerService
        , ILogger<PayServiceWeixin> logger) : base(istoreHose)
        {
            _storeHoseForWeixin = ((StorehouseEF)_storeHouse);
            _playerService = playerService;
            /// <summary>
            /// 发红包前端页面名称
            /// </summary>
            WithdrawCashViewName = "withdrawCash";
            /// <summary>
            /// 充值前端页面名称
            /// </summary>
            RechargeViewName = "recharge";
            _logger = logger;
        }
        public override PayOrder FindPayOrder(object obj)
        {
            WxPayData wxPayData = (WxPayData)obj;
            string out_trade_no = wxPayData.GetValue("out_trade_no").ToString();
          //  string openid = wxPayData.GetValue("openid").ToString();
            decimal total_fee = (int.Parse(wxPayData.GetValue("total_fee").ToString())) / 100;

            return _storeHoseForWeixin.FindPayOrder(out_trade_no);
        }
        public override PayOrder CompletePayOrder(object obj)
        {
            WxPayData wxPayData = (WxPayData)obj;
            PayOrder payOrder = FindPayOrder(wxPayData);

            if (payOrder != null && payOrder.Success == false)
            {
                Player player = _playerService.FindPlayerByName(payOrder.WeixinName);
                //IstoreHouse.GetPlayerByName(payOrder.WeixinName);
                try
                {
                    //  player_.Account.Addmount(payOrder.Amount, "充值");
                    _playerService.AdjustAccount(player, payOrder.Amount, "充值");
                    if (ManagePlayer.GetOnlyInstance().WeixinName == player.IntroducerWeixinName && payOrder.Amount <= 100)
                    {
                        _playerService.AdjustAccount(player,payOrder.Amount * (decimal)0.02, "随机奖励");
                    }
                    else
                    {
                        Player introducer = _playerService.FindPlayerByName(player.IntroducerWeixinName);
                        //IstoreHouse.GetPlayerByName(player_.IntroducerWeixinName);
                        _playerService.AdjustAccount(introducer , payOrder.Amount * (decimal)0.02, "分享奖励");
                    }
                    payOrder.Success = true;
                    _storeHouse.SaveChanges();
                    //IstoreHouse.SaveChanges();
                }
                catch (Exception)
                {

                    _logger.LogInformation("rechargeError:out_trade_no:"
                        + payOrder.Out_trade_no +
                        "_weixinName:" + payOrder.WeixinName +
                        "_amount:" + payOrder.Amount);
                }
            }
            return payOrder;
        }
        public override string GiveRedPackTo(Player player, decimal amount)
        {
            string rl;
            RedPack redPack = new RedPack();
            if (!Request(redPack, player.WeixinName, amount, "恭喜获得苹果机游戏红包", "苹果机游戏", "申请红包", "一天只能领一个红包"))
            {
               _playerService.AdjustAccount(player,amount, "红包");
                rl = "发放出现异常,一天只能领两次,请明天再试";
            }
            else
            {
                rl = "红包领取成功";
            }
            _storeHouse.AddEntity<RedPack>(redPack);
            try
            {
                _storeHouse.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Account)
                    {
                        decimal databaseValue = _storeHouse.GetAccountAsNoTracking(player.Account.Id).Balance;
                        decimal currentValue = (decimal)entry.Property("balance").CurrentValue;
                        entry.Property("balance").CurrentValue = currentValue + ((decimal)entry.Property("balance").OriginalValue - databaseValue);
                        entry.Property("balance").OriginalValue = databaseValue;
                    }
                    else
                    {
                        throw new NotSupportedException(player.Account.Id + "账户变更冲突");
                    }
                }
                _storeHouse.SaveChanges();
            }
            return rl;
        }

        public PayOrder CompletePayOrderAndReback(HttpContext httpContext, out string successStr)
        {
            ResultNotify resultNotify = new ResultNotify(httpContext);
            WxPayData WxPayData = resultNotify.ProcessNotify();
            PayOrder payOrder = null;
            successStr = "";
            if (WxPayData != null)
            {
                payOrder = CompletePayOrder(WxPayData);
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                // httpContext.Response.WriteAsync();
                successStr = res.ToXml();
            }
            return payOrder;
        }
       public string CreateWxJsApiParam(PayOrder payOrder)
        {
            JsApiPay jsApiPay = new JsApiPay();
            WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(payOrder);
            payOrder.Prepay_id = (string)unifiedOrderResult.GetValue("prepay_id");
            _storeHouse.SaveChanges();
            string wxJsApiParam = jsApiPay.GetJsApiParameters();
            return wxJsApiParam;
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
        private bool Request(RedPack redPack, string re_openid_,
                        decimal amount_,
                        string wishing_,
                        string send_name_,
                        string act_name_,
                        string remark_)
        {
            WxPayData redPackRequest = new WxPayData();
            redPackRequest.SetValue("re_openid", re_openid_);
            redPackRequest.SetValue("total_amount", (int)(amount_ * 100));
            redPackRequest.SetValue("wishing", wishing_);
            redPackRequest.SetValue("send_name", send_name_);
            redPackRequest.SetValue("act_name", act_name_);
            redPackRequest.SetValue("remark", remark_);
            WxPayData res = WxPayApi.WxRedPack(redPackRequest);
            if (res.GetValue("return_code").ToString() == "SUCCESS")
            {
                redPack.Err_code = res.GetValue("err_code").ToString();
            }
            else
            {
                redPack.Err_code = "requestFail";
            }
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
               res.GetValue("result_code").ToString() == "SUCCESS")
            {
                redPack.Send_listid = res.GetValue("send_listid").ToString();
                redPack.WeixinName = res.GetValue("re_openid").ToString();
                redPack.Amount = decimal.Parse(res.GetValue("total_amount").ToString()) / 100;
                redPack.Mch_billno = res.GetValue("mch_billno").ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
