using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{
    /// <summary>
    /// 游戏城邦(游戏城容器)
    /// </summary>
    public class GameCityCollection : List<IGameCity>
    {
       /// <summary>
       /// 游戏项目列表
       /// </summary>
        public List<IGameProject> GameProjects { get; set; }
        public GameCityCollection()
        {
            GameProjects = new List<IGameProject>();
        }
        /// <summary>
        /// 找到指定Id的房间
        /// </summary>
        /// <param name="gameCityId">游戏城Id</param>
        /// <param name="roomId">房间Id</param>
        /// <returns>房间接口</returns>
        public IRoom FindRoomByRoomId(string gameCityId, string roomId)
        {
            var gameCity_ = FindGameCityById(gameCityId);
            var room_ = gameCity_.FindRoomById(roomId);
            return room_;
        }
        /// <summary>
        /// 找到指定Id的游戏城
        /// </summary>
        /// <param name="gameCityId">游戏城</param>
        /// <returns>游戏城接口</returns>
        public IGameCity FindGameCityById(string gameCityId)
        {
            IGameCity gameCity_ = null;
            gameCity_ = this.Find(g => g.Id == gameCityId);
            return gameCity_;
        }
        public List<IRoom> FindRoomsByName(string name)
        {
            List<IRoom> rooms = new List<IRoom>();
            foreach (var item in this)
            {
                var cityRooms = item.FindRoomsByName(name);
                if (cityRooms != null)
                {
                        rooms.AddRange(cityRooms);
                }
            }
            return rooms;
        }
    
    }
}
