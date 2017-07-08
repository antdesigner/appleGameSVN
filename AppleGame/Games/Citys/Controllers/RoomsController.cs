using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AntDesigner.GameCityBase.EF;
using AntDesigner.GameCityBase.interFace;
using Microsoft.AspNetCore.Http;
using AntDesigner.NetCore.GameCity;
using Game = AntDesigner.NetCore.Games.GameSimpleCards;
using Microsoft.AspNetCore.Mvc.Rendering;
using AntDesigner.GameCityBase;
using AppleGame.Games.Citys.Models;
using AntDesigner.GameCityBase.Controllers;
using Microsoft.Extensions.DependencyInjection;
using GameCitys.GamCityBase;
using GameCitys.DomainService;


namespace AppleGame.Games.Romms.Controllers
{
    [Area("Citys")]
    [RoomIsNotExistExceptionFilter]
    public class RoomsController : CityGameController
    {
       
        public RoomsController( IHttpContextAccessor httpContextAccessor, IPlayerService playerService) : base( httpContextAccessor, playerService)
        {
            
        }

        /// <summary>
        /// /新建房间
        /// </summary>
        /// <returns>房间设置</returns>
        [HttpGet]
        public IActionResult Index_CreateRoom()
        {
            ViewBag.GameCitys = new SelectList(CityGameController.GameCityList, "Id", "Name");
            ViewBag.GameProjects = new SelectList(CityGameController.GameCityList.GameProjects, "Name", "ShowName");
            return View();
        }
        /// <summary>
        /// 新建房间
        /// </summary>
        /// <param name="createRoom"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateRoom(IFormCollection createRoom)
        {
            if (session.Keys.Contains("RoomId"))
            {
                throw new Exception("已经在一个房间里,不能再建");
            }
            if (!decimal.TryParse(createRoom["TicketPrice"].ToString(), out decimal ticketPrice_))
            {
                ticketPrice_ = 0;
            }
            string gameName_ = createRoom["gameProject"].ToString();
            IGameProject gameProject_ = LoadGameProject(gameName_);
            IInningeGame inningeGame_ = new InningeGame(gameProject_);
            gameProject_.InningeGame = inningeGame_;
            if (!int.TryParse(createRoom["PlayerCountTopLimit"].ToString(), out int limitCount_))
            {
                limitCount_ = 1;
            }
            IRoomConfig roomConfig_ = new RoomConfig(inningeGame_)
            {
                Affiche = createRoom["Affiche"],
                Name = createRoom["Name"],
                PlayerCountTopLimit = limitCount_,
                SecretKey = createRoom["SecretKey"],
                TicketPrice = ticketPrice_
            };
            IRoom room_ = new Room(player, roomConfig_);
            BoundingEventOfRoom(room_);
            var gameCityId = createRoom["gameCityId"];
            WriteToSeeion(gameCityId, room_);
            if (inningeGame_.IGameProject.PlayerCountLimit==1)
            {
                return RedirectToAction("JoinRoom");
            }
            return RedirectToAction("RoomsList");
        }
        private void BoundingEventOfRoom(IRoom room_)
        {
            room_.AddPlayerFailRoomFullEvent += AddPlayerFailRoomFullEvent_Handler;
            room_.PlayCanNotPayTicketEvent += PlayCanNotPayTicketEvent_Handler;
            room_.AddPlayer_SuccessEvent += AddPlayer_SuccessEvent_Handler;
            room_.RemovePlayer_SuccessEvent += RemovePlayer_SuccessEvent_Handler;
        }
        private void WriteToSeeion(string gameCityId , IRoom room_)
        {
            CityGameController.GameCityList.Find(g => g.Id == gameCityId).AddRoom(room_);
            session.SetString("CityId", gameCityId);
            session.SetString("RoomId", room_.Id);
        }
        /// <summary>
        /// 房间列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RoomsList()
        {
            return AllCityRooms();
        }
        protected IActionResult AllCityRooms()
        {
            List<IRoom> rooms_ = new List<IRoom>();
            foreach (var gameCity in CityGameController.GameCityList)
            {
                rooms_.AddRange(gameCity.Rooms);
            }
            ViewBag.manager = ManagePlayer.GetOnlyInstance();
            ViewBag.player = player;
            return View(rooms_);
        }
        [HttpGet]
        public IActionResult FindeRoomsByName(string name)
        {
            List<IRoom> rooms_ = new List<IRoom>();
            rooms_.AddRange(CityGameController.GameCityList.FindRoomsByName(name));
            ViewBag.manager = ManagePlayer.GetOnlyInstance();
            ViewBag.player = player;
            return View("RoomsList", rooms_);
        }
        /// <summary>
        /// 玩家进入房间
        /// </summary>
        /// <param name="RoomId">房间Id</param>
        /// <returns>游戏Index</returns>
        [HttpGet]
        public IActionResult JoinRoom()
        {
            if (!session.Keys.Contains("RoomId"))
            {
                session.SetString("CityId", _gameCity.Id);
                session.SetString("RoomId", _room.Id);
            }
            string gameName_ = null;
            gameName_ = _inngeGame.IGameProject.Name;
            if (!_room.AddPlayer(player))
            {
                return RedirectToAction("RoomsList");
            }
            //IPlayerJoinRoom roomPlayer = _room.Players.FirstOrDefault(p => p.Id == player.Id);
            //if (null != roomPlayer)
            //{
            //    roomPlayer.WebSocketLink = ClientWebsocketsManager.FindClientWebSocketByPlayerId(player.Id);
            //}
            return RedirectToAction("GameIndex", "GameHandler", new
            {
                Area = "GameProjects"
                ,
                gameCityId = _gameCity.Id,
                roomId = _room.Id
            });
        }
        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="RoomId">房间Id</param>
        /// <returns>房间列表view</returns>
        [HttpGet]
        public IActionResult LeaveRoom()
        {
            _room.RemovePlayer(player);
            session.Remove("RoomId");
            session.Remove("CityId");
            return RedirectToAction("RoomsList");
        }
        /// <summary>
        /// 踢人
        /// </summary>
        /// <param name="playerId">玩家id</param>
        [HttpGet]
        public void KickPlayer(int playerId)
        {
            if (player.Id != _room.RoomManager.Id)
            {
                return;
            }
            // _room.RemovePlayerById(playerId);
            ClientWebsocketsManager.Send(WebscoketSendObjs.LeaveRoom(playerId));
        }
        /// <summary>
        /// 添加座位比坐下加入游戏
        /// </summary>
        /// <param name="gameCityId">游戏城Id</param>
        /// <param name="roomId">房间Id</param>
        [HttpGet]
        public void SitDown()
        {
            _inngeGame.PlaySitDown(player);
        }
        /// <summary>
        /// 离开座位
        /// </summary>
        /// <param name="gameCityId">游戏城Id</param>
        /// <param name="roomId">房间Id</param>
        [HttpGet]
        public void GetUp()
        {
            ISeat seat = _inngeGame.GetSeatByPlayerId(player.Id);
            if (seat != null)
            {
                seat.PlayLeave();
            }
        }
        /// <summary>
        /// 配置房间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ConfigRoom()
        {
            if (player.Id != _room.RoomManager.Id)
            {
                return null;
            }
            ViewBag.GameProjects = new SelectList(CityGameController.GameCityList.GameProjects, "Name", "ShowName");
            return View("ConfigRoom", _room);
        }
        /// <summary>
        /// 配置房间
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ConfigRoom(ConfigRoomView config)
        {
            if (player.Id!=_room.RoomManager.Id)
            {
                return null;
                throw new Exception("不是房主没有权限修改");
            }
            ViewBag.GameProjects = new SelectList(CityGameController.GameCityList.GameProjects, "Name", "ShowName");

            _room.ChangeGame(LoadGameProject(config.GameProjectName));
            _room.Name = config.Name;
            _room.SetPlayerCountTopLimit(config.PlayerCountTopLimit);
            _room.Affiche = config.Affiche;
            _room.SetTicketPrice(config.TicketPrice);
            if (config.IsOpening == true)
            { _room.Open(); }
            else
            { _room.Close(); }
            if (config.SecretKey != null && config.SecretKey.Length > 0)
            { _room.Encrypt(config.SecretKey); }
            else
            { _room.DeEncrypt(); }
            return JoinRoom();
        }
        [HttpGet]
        public void RoomMessage(string message)
        {
            RoomMessage roomMessage = new RoomMessage(0)
            {
                Name = player.WeixinName,
                Message = message
            };

            if (null!=_room)
            {
                foreach (IPlayerJoinRoom item in _room.Players)
                {
                    if (null != item.WebSocketLink)
                    {
                        ClientWebsocketsManager.SendToWebsocket(roomMessage, item.WebSocketLink);
                    }

                }
            }

           // ClientWebsocketsManager.Send(roomMessage);
        }
    }
}
