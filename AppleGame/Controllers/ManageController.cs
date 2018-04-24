using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AntDesigner.GameCityBase.interFace;
using Microsoft.AspNetCore.Authorization;
using AntDesigner.weiXinPay;
using GameCitys.DomainService;
using GameCitys.Tools;
using AntDesigner.NetCore.GameCity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AntDesigner.GameCityBase.Controllers
{
    [Authorize(Roles = "manager")]
    public class ManageController : CityGameController
    {
        public ManageController( IHttpContextAccessor httpContextAccessor, IPlayerService playerService) : base( httpContextAccessor, playerService)
        {

        }
        public IActionResult Index()
        {

            return View("Index");
        }

        public IActionResult Index_adjustAccount(string name)
        {
            Player player = _playerService.FindPlayerByName(name);
            if (player == null)
            {
                TempData["nofind"] = name + "查找失败";
                return View("adjustAccount", base.player);
            }
            return View("adjustAccount", player);
        }
        [HttpPost]
        //public IActionResult AdjustAccount(string name_, decimal amount)
        //{
        //    string weixinName=_playerService.
        //    Player player = _playerService.FindPlayerByName(name_);
        //    _playerService.AdjustAccount(player, amount, "系统");
        //    return View("adjustAccount", player);
        //}
        public IActionResult AdjustAccount(String name_, decimal amount) {
            Player player = _playerService.FindPlayerByName(name_);
            _playerService.AdjustAccount(player, amount, "系统");
            return View("adjustAccount", player);
        }
        /// <summary>
        /// 获取全部用户列表
        /// </summary>
        /// <param name="playerService"></param>
        /// <returns></returns>
        public IActionResult AllOfPlayer([FromServices]IPlayerService playerService)
        {
            // List<Player> players = managerPlayer.SeeAllPlayers().ToList();
            IList<Player> players = playerService.GetAllPlayers();
            return View("allOfPlayers", players);
        }
        public IActionResult Index_findPlayer()
        {
            // Player player=managerPlayer.findPlayerByWeixinName(name);
            return View("findPlayer");
        }
        /// <summary>
        /// 查找玩家
        /// </summary>
        /// <param name="weixinName">名称标记</param>
        /// <param name="playerService"></param>
        /// <returns></returns>
        public IActionResult FindPlayer(int weixinName,[FromServices]IPlayerService playerService)
        {
            //Player player = managerPlayer.FindPlayerByWeixinName(weixinName);

            Player player = playerService.FindPlayerByAccountId(weixinName);
            return View("findPlayer", player);
        }
        public IActionResult Index_newNotice()
        {

            return View("newNotice");
        }
        /// <summary>
        /// 添加新公告
        /// </summary>
        /// <param name="content">公告内容</param>
        /// <param name="stopTheGame">关闭游戏</param>
        /// <param name="noticeService"></param>
        [HttpPost]
        public void AddNewNotice(string content, bool stopTheGame,[FromServices]INoticeService noticeService)
        {
            Notice notice = new Notice(content);
            //managerPlayer.PublishNotice(notice);
            //IstoreHouse.SaveChanges();
            noticeService.PublishNotice(notice);
            GameCity.IsColsed = true;
            return;
        }
        /// <summary>
        /// 公告列表
        /// </summary>
        /// <param name="noticeService"></param>
        /// <returns></returns>
        public IActionResult NoticesManager([FromServices]INoticeService noticeService)
        {
            // var notices = IstoreHouse.GetAllNotices();
            var notices = noticeService.AllNotices();
            return View("notices", notices);
        }
        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="noticeId">公告Id</param>
        /// <param name="noticeService"></param>
        [HttpPost]
        public void DeleteNotice(int noticeId, [FromServices]INoticeService noticeService)
        {
            Notice notice = noticeService.GetNoticeById(noticeId);
            noticeService.RemoveNotice(notice);
           // Notice notice = IstoreHouse.GetEntityById<Notice>(noticeId);
            //managerPlayer.RemoveNotice(notice);
            //IstoreHouse.SaveChanges();

            return;
        }
        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="noticeId">公告Id</param>
        /// <param name="newContent">公告新内容</param>
        /// <param name="noticeService"></param>
        [HttpPost]
        public void ModifyNotice(int noticeId, string newContent,[FromServices]INoticeService noticeService)
        {
            //Notice notice = IstoreHouse.GetEntityById<Notice>(noticeId);
            //managerPlayer.ModifyNotice(notice, newContent);
            //IstoreHouse.SaveChanges();
            Notice notice = noticeService.GetNoticeById(noticeId);
            noticeService.ModifyNotice(notice, newContent);
            return;
        }
        //public IActionResult PlayersOnline() {
        //    List<Player> playersOnline = managerPlayer.SeeOnliePlayers().ToList();
        //    return View("playersOnline", playersOnline);
        //}
        [HttpGet]
        public IActionResult Index_sendMessageTo(string name)
        {
            ViewBag.receiverName = name;
            ViewBag.content = "";
            return View("sendMessage");
        }
        [HttpPost]
        public IActionResult SendMessage(string content, string receiverName, [FromServices]IMessageService messageService)
        {
            //Player player = managerPlayer.FindPlayerByWeixinName(receiverName);
            //managerPlayer.SendMessageTo(player, content);
            //IstoreHouse.SaveChanges();
            Player receiver = _playerService.FindPlayerByName(receiverName);
            Message message = new Message(ManagePlayer.GetOnlyInstance(), content, receiver);
            messageService.SendMessage(message);
            ViewBag.receiverName = receiverName;
            ViewBag.content = content;
            TempData["sendSuccess"] = "发送完成";
            return View("sendMessage");
        }
        public IActionResult Messages(string name,[FromServices]IMessageService messageService)
        {
            // Player player = managerPlayer.FindPlayerByWeixinName(name);
            IList<Message> messages = messageService.Messages(name);
            return View("../Message/messages", messages);
        }
        public IActionResult AccountDetail(string name)
        {
            // Player player = managerPlayer.FindPlayerByWeixinName(name);
           var  player = _playerService.FindPlayerByName(name);
            IList<AccountDetail> accountDetails = _playerService.GetAccountDetailsOfPlayer(player);
            ViewBag.account = player.Account;
            return View("../Account/accountDetail", accountDetails);
        }
        public IActionResult SetGameDegree(string degree)
        {
            managerPlayer.SetGameDegree(degree);
            return View("Index");
        }
        public IActionResult Index_redPackges()
        {
            return View("Index_redPackges");
        }
        public IActionResult RedPackges(string fromDateStr, string toDateStr,[FromServices]IPayService payService)
        {
            ToolsData.DataFromToOfStrToDateTime(fromDateStr, toDateStr, out DateTime fromDate, out DateTime toDate);
            IList<RedPack> redPackges = payService.GetRedPackgeList(fromDate,toDate);
            //IList<RedPack> redPackges = managerPlayer.GetRedPackgeList(fromDate, toDate);
            return View("redPackges", redPackges);
        }
        /// <summary>
        /// 查询充值记录
        /// </summary>
        /// <returns></returns>
        public IActionResult Index_rechargeOrders()
        {
            return View("Index_rechargeOrders");
        }
        /// <summary>
        /// 返回查询的支付订单
        /// </summary>
        /// <param name="fromDateStr">开始日期</param>
        /// <param name="toDateStr">结束日期</param>
        /// <param name="payService"></param>
        /// <returns></returns>
        public IActionResult RechargeOrders(string fromDateStr, string toDateStr, [FromServices]IPayService payService)
        {
            ToolsData.DataFromToOfStrToDateTime(fromDateStr, toDateStr, out DateTime fromDate, out DateTime toDate);
            //IList<PayOrder> rechargeOrders = managerPlayer.GetPayOrderList(fromDate, toDate);
            IList<PayOrder> rechargeOrders = payService.GetPayOrderList(fromDate, toDate);
            return View("rechargeOrders", rechargeOrders);
        }
      
        public IActionResult Index_CreatGameCity()
        {

        return View("Index_CreatGameCity");
        }
        /// <summary>
        /// 新建游戏城
        /// </summary>
        /// <param name="weixinName">城主微信码</param>
        /// <param name="cityName">名称</param>
        /// <returns></returns>
        public IActionResult CreatGameCity([FromServices]IGameCityService gameCityService, string weixinName = "", string cityName = "游戏城")
        {
            Player cityManager = _playerService.FindPlayerByName(weixinName);
            gameCityService.CreatGameCity(CityGameController.GameCityList,cityManager, cityName);
            return RedirectToAction("Index", "Citys", new {Area="Citys"});
        }
    }
}
