using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using AntDesigner.GameCityBase.Controllers;
using AntDesigner.GameCityBase.interFace;
using Microsoft.AspNetCore.Http;
using AntDesigner.GameCityBase.boxs;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using AntDesigner.GameCityBase;
using System.Collections.ObjectModel;
using System.Text;
using WxPayAPI;
using AntDesigner.weiXinPay;
using AntDesigner.NetCore.GameCity;
using GameCitys.DomainService;
using GameCitys.Tools;
using GameCitys.GamCityBase;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppleGame.Games.GameProjects.Controllers
{
    [Area("GameProjects")]
    public class GameHandlerController : CityGameController
    {
        public GameHandlerController( IHttpContextAccessor httpContextAccessor, IPlayerService playerService) : base( httpContextAccessor, playerService)
        {
        }
        [HttpGet]
        public override IActionResult GameIndex()
        {
            StringBuilder url = new StringBuilder();
            url.Append(WxPayConfig.SiteName + "/Game/loginGame?weixinName=");
            url.Append(player.WeixinName);
            url.Append("&shareId=");
            url.Append(ToolsSecret.EncryptOpenId(player.IntroducerWeixinName));
            ViewBag.wxConfig = new wxConfig(url.ToString());
            ViewBag.manager = ManagePlayer.GetOnlyInstance();
            ViewBag.Player = player;
            ViewBag.shareId = ToolsSecret.EncryptOpenId(player.WeixinName);
            ViewBag.accesstoken = WxPayConfig._access_token.Access_token;
            ViewBag.jsToken = WxPayConfig._jsapi_ticket.Ticket;
            var iGameProject = _room.InningGame.IGameProject;
            ViewBag.GameProject = iGameProject;
            var gameName = iGameProject.Name;
            return View(gameName+"GameIndex", _room);

        }
    }
}
