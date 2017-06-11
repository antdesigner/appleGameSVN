using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGameCity
    {
        /// <summary>
        /// 游戏城Id
        /// </summary>
        string Id { get; }
        /// <summary>
        /// 城主
        /// </summary>
        ICityManager ICityManager { get; }
        /// <summary>
        /// 公告
        /// </summary>
        INotice INotic { get; set; }
        /// <summary>
        /// 开放
        /// </summary>
        bool IsOpening { get; set; }
        /// <summary>
        /// 游戏城名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 游戏城的房间
        /// </summary>
        List<IRoom> Rooms { get; }
        /// <summary>
        /// 进入密码
        /// </summary>
        string SecretKey { get; set; }
        /// <summary>
        /// 开房价格
        /// </summary>
        decimal TicketPrice { get; set; }
        /// <summary>
        /// 委托检查能否开房
        /// </summary>
        Func<IRoom, bool> DAddRoomChek { get; set; }
        /// <summary>
        /// 检查凭据
        /// </summary>
        Func<IJoinGameCityTicket, bool> DCheckTicket { get; set; }
        /// <summary>
        /// 进入房间后事件
        /// </summary>
        event EventHandler AfterAddRoomHandler;
        /// <summary>
        /// 进入房间前事件
        /// </summary>
        event EventHandler BeforDeleteRoomHandler;
        /// <summary>
        /// 开房钱不够事件
        /// </summary>
        event EventHandler FailAddRoomNotEnoughMoney;
        Action<IRoom> DPlayCreateRoomFail { get; set; }
        /// <summary>
        /// 添加随机房间
        /// </summary>
        /// <param name="inningGame_"></param>
        /// <param name="playerCount_"></param>
        /// <param name="n"></param>
        void AddAutoRoom(IInningeGame inningGame_, int playerCount_, int n);
       /// <summary>
       /// 玩家进入
       /// </summary>
       /// <param name="ticket_">凭据</param>
       /// <returns>YN</returns>
        bool AddPlayer(IJoinGameCityTicket ticket_);
        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="room_"></param>
        /// <returns></returns>
        bool AddRoom(IRoom room_);
        /// <summary>
        /// 找到指定Id的房间
        /// </summary>
        /// <param name="id_"></param>
        /// <returns></returns>
        IRoom FindRoomById(string id_);
        IList<IRoom> FindRoomsByName(string name);
        /// <summary>
        /// 随机提供一个免费房间
        /// </summary>
        /// <returns></returns>
        IRoom ProvideRandomFreeRoom();
        /// <summary>
        /// 随机提供一个房间
        /// </summary>
        /// <returns></returns>
        IRoom ProvideRandomRoom();
        /// <summary>
        /// 删除房间
        /// </summary>
        /// <param name="room_"></param>
        /// <returns></returns>
        bool RemoveRoom(IRoom room_);
    }

   
}
