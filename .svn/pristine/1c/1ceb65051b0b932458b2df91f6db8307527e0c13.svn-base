using System;

namespace AntDesigner.NetCore.GameCity
{
    public interface IGameCityConfig
    {
        /// <summary>
        /// 进入游戏城检查
        /// </summary>
        Func<IJoinGameCityTicket, bool> DCheckTicket { get; set; }
        /// <summary>
        /// 公告
        /// </summary>
        INotice INotic { get; set; }
        /// <summary>
        /// 游戏城开放状态
        /// </summary>
        bool IsOpening { get; set; }
        /// <summary>
        /// 游戏城名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// /游戏城密码
        /// </summary>
        string SecretKey { get; set; }
        /// <summary>
        /// 建立房间的价格
        /// </summary>
        decimal TicketPrice { get; set; }
        /// <summary>
        /// 能否添加房间的检查
        /// </summary>
       Func<IRoom, bool> DAddRoomChek { get; set; }
    }
}