
using System;
using GameCitys.DomainService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WxPayAPI;
using Microsoft.Extensions.Logging;

namespace AntDesigner.GameCityBase.Controllers
{
    public class MyController : Controller
    {
        protected ISession session;
        protected IHttpContextAccessor httpContextAccessor;
        protected ManagePlayer managerPlayer;
        protected Player player;
        protected IPlayerService _playerService;
        static MyController()
        {
  
        }



        public MyController(IHttpContextAccessor httpContextAccessor_
            , IPlayerService playerService)
        {
            _playerService = playerService;
            httpContextAccessor = httpContextAccessor_;
            session = httpContextAccessor_.HttpContext.Session;
            ManagePlayer.DgetManagePlayer = playerService.GetManagerPlayer;
            LoadManager();
            playerService.AttachPlayer(ManagePlayer.GetOnlyInstance());
            LoadPlayerInfo();
            Player.DDecutMoney = DecutMoney;
        }
        private void LoadManager()
        {
            if (session.GetInt32("playerId") == ManagePlayer.GetOnlyInstance().Id)
            {
                managerPlayer = ManagePlayer.GetOnlyInstance();
            }
            else
            {
                managerPlayer = null;
            }
        }
        protected virtual void LoadPlayerInfo()
        {
            var playerId = session.GetInt32("playerId");
            if (playerId == null)
            {
                return;
            }
            if (playerId > 0 && playerId != ManagePlayer.GetOnlyInstance().Id)
            {
                player = _playerService.FindPlayerByName(session.GetString("playerName"));
            }
            else if (playerId == ManagePlayer.GetOnlyInstance().Id)
            {
                player = ManagePlayer.GetOnlyInstance();
            }
        }
        private decimal DecutMoney(Player Dplayer, decimal amount, string cause)
        {
            Player myPlayer = _playerService.FindPlayerByName(Dplayer.WeixinName);
            _playerService.AdjustAccount(myPlayer, -amount, cause);
            return myPlayer.Account.Balance;
        }
        protected bool IsWeixinSeverIp(ILogger<GameController> logger) {
            string clientIp = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var weixinIps = WxPayConfig.GetWeixinIps();
            var ips = weixinIps.ip_list;
            logger.LogWarning("客户端Ip:" + clientIp);
            for (int i = 0; i < ips.Length; i++) {
                logger.LogWarning("微信severIps:" + ips[i]);
                if (ips[i] == clientIp) {
                    return true;
                }
            }
            return false;
        }
    }
}
