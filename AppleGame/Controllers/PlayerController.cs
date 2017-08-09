using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AntDesigner.GameCityBase.EF;
using AntDesigner.GameCityBase;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AntDesigner.GameCityBase.interFace;
using WxPayAPI;
using AntDesigner.weiXinPay;
using System.IO;
using System.Text;
using AntDesigner.NetCore.GameCity;
using GameCitys.DomainService;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AntDesigner.GameCityBase.Controllers
{
 
    public class PlayerController : MyController
    {

        public PlayerController( IHttpContextAccessor httpContextAccessor, IPlayerService playerService) : base( httpContextAccessor, playerService)
        {

        }
        public IActionResult Index()
        {
            return View("friends");
        }
        public IActionResult Friends([FromServices]IPlayerService playerService)
        {
            IList<Player> friends = playerService.GetPlayersOfPlayer(player);
            return View("friends", friends);
        }
        public IActionResult Index_withdrawCash([FromServices]IPayService payService)
        {
            string viewName = payService.WithdrawCashViewName;
            // return View("withdrawCash", player.Account);
            return View(viewName, player.Account);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult WithdrawCash(decimal amount, [FromServices]IPayService payService)
        {
            string viewName = payService.WithdrawCashViewName;
            if (amount > 30)
            {
                return View(viewName, player.Account);
            }
            try
            {
                _playerService.AdjustAccount(player,-amount, "红包");
            }
            catch (Exception e)
            {
                ViewBag.result = e.Message;
                return View(viewName, player.Account);
            }
            #region old
            //RedPack redPack = new RedPack();
            //if (!redPack.Request(player.WeixinName, amount, "恭喜获得苹果机游戏红包", "苹果机游戏", "申请红包", "一天只能领一个红包"))
            //{
            //    ViewBag.result = "发放出现异常,一天只能领两次,请明天再试";
            //    player.Account.Addmount(amount, "红包");
            //}
            //else {
            //    ViewBag.result = "红包领取成功";
            //}
            //IstoreHouse.AddEntity<RedPack>(redPack);
            //try
            //{
            //    IstoreHouse.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    foreach (var entry in ex.Entries)
            //    {
            //        if (entry.Entity is Account)
            //        {
            //            decimal  databaseValue = IstoreHouse.GetAccountAsNoTracking(player.Account.Id).Balance;
            //            decimal currentValue =(decimal)entry.Property("balance").CurrentValue;

            //            entry.Property("balance").CurrentValue = currentValue+((decimal)entry.Property("balance").OriginalValue - databaseValue);
            //            entry.Property("balance").OriginalValue = databaseValue;
            //        }
            //        else
            //        {
            //            throw new NotSupportedException(player.Account.Id +"账户变更冲突" );
            //        }
            //    }
            //    IstoreHouse.SaveChanges();

            // }
            #endregion
            ViewBag.result = payService.GiveRedPackTo(player, amount);
            return View(viewName, player.Account);
        }
        public IActionResult Index_recharge([FromServices]IPayService payService)
        {
            string viewName = payService.RechargeViewName;
            return View(viewName, player.Account);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatOrder(decimal amount,[FromServices]IPayServiceWeixin payServiceWeixin)
        {
            string viewName = payServiceWeixin.RechargeViewName;
            if (amount>10)
            {
                return View(viewName, player.Account);
            }
            var order= payServiceWeixin.CreatPayOrder(player, amount);
            ViewBag.wxJsApiParam =payServiceWeixin.CreateWxJsApiParam(order);
            ViewBag.amount = amount;
            return View(viewName, player.Account);
            //新建订单
        }
  
    }

}

