﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AntDesigner.GameCityBase;
using AntDesigner.GameCityBase.interFace;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using AntDesigner.GameCityBase.boxs;
using System.Collections.ObjectModel;
using AntDesigner.weiXinPay;
using WxPayAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;
using GameCitys.DomainService;
using GameCitys.Tools;
using Microsoft.Extensions.Logging;
using AntDesigner.NetCore.GameCity;
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace AntDesigner.GameCityBase.Controllers {

    [Authorize]
    public class GameController : MyController {

        public GameController(IHttpContextAccessor httpContextAccessor, IPlayerService playerService) : base(httpContextAccessor, playerService) {


        }
        public IActionResult GameExplain() {
            return View("gameExplain");
        }
        [AllowAnonymous]
        public IActionResult Index(string state) {
            if (state != null && state != "") {

                Player player_ = _playerService.FindPlayerByName(ToolsSecret.DecryptOpenId(state));
                if (player_ != null) {
                    ViewBag.shareId = ToolsSecret.EncryptOpenId(player_.WeixinName);
                }
            }
            else {
                ViewBag.shareId = ToolsSecret.EncryptOpenId(ManagePlayer.GetOnlyInstance().WeixinName);
            }

            return View("Index");

        }
        [AllowAnonymous]
        public IActionResult Robits() {
            return View("robits");
        }
        [AllowAnonymous]
        public IActionResult LoginGame(string weixinName, string shareId, [FromServices]ILoginGame Ilogin) {
            Ilogin.DaddPlayer = _playerService.AddPlayer;
            Ilogin.DgetPlayerByWeixianName = _playerService.FindPlayerByName;
            if (shareId == null || shareId == "") {
                shareId = ToolsSecret.EncryptOpenId(ManagePlayer.GetOnlyInstance().WeixinName);
            }
            Player player = Ilogin.Login(weixinName, ToolsSecret.DecryptOpenId(shareId));
            if (player != null && base.player != null && base.player.WeixinName != player.WeixinName) {

                return View("Index");
            }
            SavePlayerInfoInSession(player);
            base.LoadPlayerInfo();
            Sigin(player);
            BuiderShareLink(player);
            if (GameCity.IsColsed&&player.Id!= ManagePlayer.GetOnlyInstance().Id) {
                return RedirectToAction("ShowNotice");
            }
            return RedirectToAction("RoomsList", "Rooms", new { Area = "Citys" });
        }
        [AllowAnonymous]
        public IActionResult LoginMyGame(string weixinName, string checkK, string shareId, [FromServices]ILoginGame Ilogin) {
            if (checkK != WxPayConfig.CheckK) {
                return View("Index");
            }
            Ilogin.DaddPlayer = _playerService.AddPlayer;
            Ilogin.DgetPlayerByWeixianName = _playerService.FindPlayerByName;
            if (shareId == null || shareId == "") {
                shareId = ToolsSecret.EncryptOpenId(ManagePlayer.GetOnlyInstance().WeixinName);
            }
            Player player = Ilogin.Login(weixinName, ToolsSecret.DecryptOpenId(shareId));
            if (player != null && base.player != null && base.player.WeixinName != player.WeixinName) {

                return View("Index");
            }
            SavePlayerInfoInSession(player);
            base.LoadPlayerInfo();
            Sigin(player);
            BuiderShareLink(player);
            return RedirectToAction("RoomsList", "Rooms", new { Area = "Citys" });
        }
        private void BuiderShareLink(Player player) {
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
        }
        [AllowAnonymous]
        public IActionResult GetWeixinMessage([FromServices]ILogger<GameController> logger) {
            if (httpContextAccessor.HttpContext.Request.Method.ToUpper() == "POST") {

            }
            else if (httpContextAccessor.HttpContext.Request.Method.ToUpper() == "GET") {
                bool IsWeiXinServerRequest = IsWeixinSeverIp(logger);
                if (!IsWeiXinServerRequest) {
                    return Content("");
                }
                string signature = httpContextAccessor.HttpContext.Request.Query["signature"];
                string timestamp = httpContextAccessor.HttpContext.Request.Query["timestamp"];
                string nonce = httpContextAccessor.HttpContext.Request.Query["nonce"];
                string echostr = httpContextAccessor.HttpContext.Request.Query["echostr"];
                if (LoginByWeixin.CheckedSignature(timestamp, nonce, signature)) {
                    return Content(echostr);
                }
            }
            return Content("");
        }
        [AllowAnonymous]
        public IActionResult LoginByWeixin_(string code, string state, [FromServices]ILoginGame Ilogin) {

            string weixinName_ = LoginByWeixin.GetOpenId(code);
            return RedirectToAction("loginGame", new { weixinName = weixinName_, shareId = state });

        }
        public IActionResult LogGameAgain() {

            if (player != null) {
                return RedirectToAction("loginGame", new { weixinName = player.WeixinName, shareId = ToolsSecret.EncryptOpenId(player.IntroducerWeixinName) });
            }

            return View("Index");
        }
        private async void Sigin(Player player_) {
            ClaimsPrincipal userPrincipal = CreatePrincipal(player_);
            await httpContextAccessor.HttpContext.Authentication.SignInAsync("MyCookieMiddlewareInstance", userPrincipal,
                new AuthenticationProperties {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(360),
                    IsPersistent = false,
                    AllowRefresh = false
                });

        }
        private static ClaimsPrincipal CreatePrincipal(Player player_) {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, player_.WeixinName, ClaimValueTypes.String)
            };
            string role = "player";
            if (player_.Id == ManagePlayer.GetOnlyInstance().Id) {
                role = "manager";
            }
            claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String));
            var userIdentity = new ClaimsIdentity(role);
            userIdentity.AddClaims(claims);
            var userPrincipal = new ClaimsPrincipal(userIdentity);
            return userPrincipal;
        }
        private void SavePlayerInfoInSession(Player player) {
            session.SetInt32("playerId", player.Id);
            session.SetString("playerName", player.WeixinName);
            session.SetInt32("playerAccountId", player.Account.Id);
        }
        public IActionResult ShowNotice([FromServices]INoticeService noticeService) {
            ViewBag.notice = noticeService.GetNotices(1)[0];
            return View("Notice");
        }

    }
}
