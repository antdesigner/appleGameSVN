
using AntDesigner.NetCore.GameCity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using AntDesigner.NetCore.Games.GameSimpleCards;
using AntDesigner.NetCore.Games.GameTiger;
using AntDesigner.NetCore.Games.GameThreePokers;
using AntDesigner.NetCore.Games.GameMajiangDaoDaoHu;
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
        /// <summary>
        /// 房间加载游戏
        /// </summary>
        /// <param name="gameName_">游戏名称</param>
        /// <returns></returns>
        protected IGameProject LoadGameProject(string gameName_)
        {
            Assembly assembly = Assembly.Load(new AssemblyName(gameName_));
            IGameProject gameProject_ = (IGameProject)assembly.CreateInstance("AntDesigner.NetCore.Games." + gameName_ + "." + gameName_);
            gameProject_.Notify = ClientWebsocketsManager.Send;
            gameProject_.NotifyByWebsockLink = ClientWebsocketsManager.SendToWebsocket;
            gameProject_.DChangePlayerAccount = PlayerService.AdjustAccountForDelegate;
            return gameProject_;
        }

        public CityGameController( IHttpContextAccessor httpContextAccessor_, IPlayerService playerService) : base(httpContextAccessor_, playerService)
        {
            
            LoadRoomInfo();
        }
      
        /// <summary>
        /// 检查玩家session保存的房间Id等,初始化房间信息
        /// </summary>
        private void LoadRoomInfo()
        {
            string gameCityId_;
            string roomId_;
            #region session中保存有房间信息就读取,没有就从Request.Query中读取
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
            #endregion
            #region sesson或Request.Query中有房间信息就使用
            if (gameCityId_ != null && gameCityId_.Length > 0)
            {
                _gameCity = CityGameController.GameCityList.FindGameCityById(gameCityId_);
            }
            if (roomId_ != null && roomId_.Length > 0)
            {
                _room = _gameCity.FindRoomById(roomId_);
                if (_room == null)
                {
  throw new RoomIsNotExistException(player.Id, "房间已经不存在了");
                }
                _inngeGame = _room.InningGame;
                _gameProject = _inngeGame.IGameProject;
#endregion
            #region 保存玩家websocket对象
                IPlayerJoinRoom roomPlayer = _room.Players.FirstOrDefault(p => p.Id == player.Id);
                if (null != roomPlayer)
                {
                    roomPlayer.WebSocketLink = ClientWebsocketsManager.FindClientWebSocketByPlayerId(player.Id);
                }
#endregion
            }
        }
        /// <summary>
        /// 创建游戏城容器
        /// </summary>
        static CityGameController()
        {
            GameCityList = new GameCityCollection();
            LoadGameProjects();
        }
        /// <summary>
        /// 装载游戏项目,以供新建房间时选择项目
        /// </summary>
        private static void LoadGameProjects() {
            GameCityList.GameProjects.Add(new GameThreePokers());
           // GameCityList.GameProjects.Add(new GameSimpleCards());
            GameCityList.GameProjects.Add(new GameTiger());
           // GameCityList.GameProjects.Add(new GameMajiangDaoDaoHu());
          
        }
        /// <summary>
        /// 玩家离开房间后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void _room_RemovePlayer_SuccessEvent(object sender, EventArgs e)
        {
            IRoom room = (IRoom)sender;
            if (room.Players.Count == 0)
            {
                IGameCity gameCity = GameCityList.FindGameCityById(room.GameCityId);
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
            return View(gameName + "GameIndex", _room);
        }
        /// <summary>
        /// SimpleCards游戏开始
        /// </summary>
        /// <param name="roomId"></param>
        [HttpGet]
        public void Start()
        {
            var innigeGame = _room.InningGame;
            if (!innigeGame.IsStarted)
            {
                innigeGame.Start();
            }else if(innigeGame.IsGameOver) {
                innigeGame.Reset();
            }
            ViewBag.GameProject = _room.InningGame.IGameProject;
            ViewBag.Player = player;
        }
        /// <summary>
        ///  player.websocket连接激活
        /// </summary>
        [HttpGet]
        public void WebsocketCheck()
        {
            
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
            if (_inngeGame!=null&&_inngeGame.IsStarted!=true)
            {
                ClientWebsocketsManager.Send(new Alert(player.Id, "游戏还未启动!请房主先启动游戏"));
                return null;
            }
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
