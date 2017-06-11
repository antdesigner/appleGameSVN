using AntDesigner.GameCityBase;
using AntDesigner.GameCityBase.boxs;
using AntDesigner.GameCityBase.EF;
using AntDesigner.GameCityBase.interFace;
using AntDesigner.NetCore.GameCity;
using AntDesigner.weiXinPay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WxPayAPI;
using AntDesigner.NetCore.Games.GameSimpleCards;
using AntDesigner.GameCityBase.Controllers;
using AntDesigner.NetCore.Games.GameTiger;
using System.Reflection;
using GameCitys.GamCityBase;
using GameCitys.DomainService;
using Microsoft.Extensions.DependencyInjection;
namespace AntDesigner.GameCityBase.Controllers
{
    public class CityGameController : MyController
    {
        protected static GameCityCollection GameCityList;
        protected IGameCity _gameCity;
        protected IRoom _room;
        protected IInningeGame _inngeGame;
        protected IGameProject _gameProject;
        protected IGameProject LoadGameProject(string gameName_)
        {
            Assembly assembly = Assembly.Load(new AssemblyName(gameName_));
           
            IGameProject gameProject_ = (IGameProject)assembly.CreateInstance("AntDesigner.NetCore.Games." + gameName_ + "." + gameName_);
            gameProject_.Notify= ClientWebsocketsManager.Send;
            // gameProject_.DChangePlayerAccount = _playerService.AdjustAccount;
            gameProject_.DChangePlayerAccount = PlayerService.AdjustAccountForDelegate;
            return gameProject_;
        }
        public CityGameController( IHttpContextAccessor httpContextAccessor_,IPlayerService playerService) : base( httpContextAccessor_,playerService)
        {
            string gameCityId_;
            string roomId_;
            if (session.Keys.Contains("RoomId") && session.Keys.Contains("CityId"))
            {
                 gameCityId_ = session.GetString("CityId");
             roomId_ = session.GetString("RoomId");
            }
            else
            {
              gameCityId_ = httpContextAccessor.HttpContext.Request.Query["gameCityId"];
               roomId_ = httpContextAccessor.HttpContext.Request.Query["roomId"];
            }

           
            if (gameCityId_!= null && gameCityId_.Length > 0)
            {
                _gameCity = CityGameController.GameCityList.FindGameCityById(gameCityId_);
            }
            if (roomId_ != null && roomId_.Length > 0)
            {
                _room = _gameCity.FindRoomById(roomId_);
                if (_room==null)
                {
                    throw new RoomIsNotExistException(player.Id,"房间已经不存在了");
                }
                _inngeGame = _room.InningGame;
                _gameProject = _inngeGame.IGameProject;
              
            }
        }
        static CityGameController()
        {
            GameCityList = new GameCityCollection();
            GameCityList.GameProjects.Add(new GameSimpleCards());
            GameCityList.GameProjects.Add(new GameTiger());
           
        }
        /// <summary>
        /// 玩家离开房间后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
  protected void _room_RemovePlayer_SuccessEvent(object sender, EventArgs e)
        {
            IRoom room = (IRoom)sender;
            if (room.Players.Count==0)
            {
             IGameCity gameCity=   GameCityList.FindGameCityById(room.GameCityId);
             gameCity.RemoveRoom(room);
            }
        }
        /// <summary>
        /// 玩家进入房间成功后事件
        /// </summary>
        /// <param name="sender"></param>
      protected void Room_AddPlayerSuccessEvent(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 游戏界面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual IActionResult GameIndex()
        {
            var iGameProject = _room.InningGame.IGameProject;
            ViewBag.GameProject = iGameProject;
            ViewBag.Player = player;
            string gameName = iGameProject.Name;
         //   return View("GameIndex", _room);
            return View(gameName+"GameIndex", _room);
        }
        /// <summary>
        /// SimpleCards游戏开始
        /// </summary>
        /// <param name="roomId"></param>
        [HttpGet]
        public void  Start()
        {

           if (!_room.InningGame.IsStarted)
            {
               _room.InningGame.Start();
            }
            ViewBag.GameProject = _room.InningGame.IGameProject;
            ViewBag.Player = player;

        }
        /// <summary>
        /// 客户端ajax请求服务端方法
        /// </summary>
        /// <param name="askMethodName">GamProjec方法名称</param>
        /// <param name="methodParam">参数</param>
        /// <returns></returns>
        [HttpPost]
        public virtual IActionResult WebSocketHandler(string askMethodName, Dictionary<string, string> methodParam)
        {
            methodParam.Remove("askMethodName");
            methodParam.Add("playerId", player.Id.ToString());
            IGameProject gameProject = _room.InningGame.IGameProject;
            string respondStr =  gameProject.ClinetHandler(askMethodName, methodParam);
            return Content(respondStr);
        }
        /// <summary>
        /// 玩家离开房间成功后事件
        /// </summary>
        /// <param name="innigeGame"></param>
        /// <param name="e">包含属性IPlayJoinRoom</param>
        protected void RemovePlayer_SuccessEvent_Handler(object room, EventArgs e)
        {
            IRoom room_ =(IRoom)room;
            if (room_.Players.Count==0)
            {
            var gameCity_=GameCityList.First(c => c.Id == room_.GameCityId);
                gameCity_.RemoveRoom(room_);
            }
        }
        /// <summary>
        /// 玩家进入房间成功后事件
        /// </summary>
        /// <param name="innigeGame"></param>
        /// <param name="e">包含属性IPlayJoinRoom</param>
        protected void AddPlayer_SuccessEvent_Handler(object room, EventArgs e)
        {
        }
        /// <summary>
        /// 玩家账户不足支付开房费用事件
        /// </summary>
        /// <param name="innigeGame"></param>
        /// <param name="e">包含属性IPlayJoinRoom</param>
        protected void PlayCanNotPayTicketEvent_Handler(object room, EventArgs e)
        {
        }
        /// <summary>
        /// 房间已满玩家加入失败事件
        /// </summary>
        /// <param name="innigeGame"></param>
        /// <param name="e">包含属性IPlayJoinRoom</param>
        protected void AddPlayerFailRoomFullEvent_Handler(object room, EventArgs e)
        {
        }

    }
}
