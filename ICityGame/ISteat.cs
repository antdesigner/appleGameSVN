using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.GameCity
{
    /// <summary>
    /// 座位
    /// </summary>
    public interface ISeat
    {
        /// <summary>
     /// 座位上没人
     /// </summary>
        bool IsEmpty { get; }
        /// <summary>
        /// 玩家
        /// </summary>
        IPlayerJoinRoom IPlayer { get; }
        /// <summary>
        /// 一局游戏
        /// </summary>
        IInningeGame IIningeGame { get; }
        /// <summary>
        /// 该座位玩家游戏中各种数据
        /// </summary>
        Dictionary<string, List<string>> GameDateStr { get; set; }
        /// <summary>
        /// 该座位玩家游戏中各种数据
        /// </summary>
        Dictionary<string, List<object>> GameDateObj { get; set; }
        /// 玩家是否能坐下
        /// </summary>
        Func<IInningeGame, bool> DCheckSitDown { get; set; }
        /// <summary>
        /// 玩家坐下前事件
        /// </summary>
        event EventHandler BeforSitDownHandler;
        /// <summary>
        /// 玩家坐下
        /// </summary>
        /// <param name="player_">玩家</param>
        /// <returns></returns>
        bool PlayerSitDown(IPlayerJoinRoom player_);
        /// <summary>
        /// 玩家做下后事件
        /// </summary>
        event EventHandler AfterSitDownHandler;
        /// <summary>
        /// 玩家离开座位前事件
        /// </summary>
        event EventHandler BeforPlayerLeaveHandler;
        /// <summary>
        /// 玩家离开座位
        /// </summary>
        void PlayLeave();
        /// <summary>
        /// 玩家离开座位后事件
        /// </summary>
        event EventHandler AfterPlayerLeaveHandler;
        /// <summary>
        /// 清空游戏数据
        /// </summary>
        void ClearSeatInfo();
    }
}
