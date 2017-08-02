using System;
using System.Collections.Generic;

namespace AntDesigner.NetCore.GameCity
{
    public interface IInningeGame
    {
   
        /// <summary>
        /// 一种游戏
        /// </summary>
        IGameProject IGameProject { get; set; }
        /// <summary>
        /// 游戏开始了?
        /// </summary>
        bool IsStarted { get; }
        /// <summary>
        /// 被异常中断了
        /// </summary>
        bool IsStoped { get; }
        /// <summary>
        /// 正常结束状态 状态设计模式?
        /// </summary>
        bool IsGameOver { get; }
        /// <summary>
        /// 座位数量
        /// </summary>
        int SeatCount { get; }
        /// <summary>
        /// 房间
        /// </summary>
        IRoom IRoom { get; set; }
        /// <summary>
        /// 能否加座位检查
        /// </summary>
        Func<IInningeGame, bool> DCheckAddSeat { get; set; }
        /// <summary>
        /// 添加座位委托
        /// </summary>
        Func<IInningeGame, ISeat> DCreatSeat { get; set; }
        /// <summary>
        /// 添加座位前事件
        /// </summary>
        event EventHandler BeforAddSeatHandler;
        /// <summary>
        /// 添加座位后事件
        /// </summary>
        event EventHandler AfterAddSeatHandler;
        /// <summary>
        /// 启动游戏前事件
        /// </summary>
        event EventHandler BeforGameStartHandler;
        /// <summary>
        /// 能否启动游戏检查
        /// </summary>
        Func<IInningeGame, bool> DCheckStart { get; set; }
        /// <summary>
        /// 游戏启动事件
        /// </summary>
        event EventHandler GameStartHandler;
        /// <summary>
        /// 游戏异常中断事件
        /// </summary>
        event EventHandler StoptedHandler;
        /// <summary>
        /// 游戏正常结束事件
        /// </summary>
        event EventHandler GameOverHander;
        /// <summary>
        ///重置游戏后事件
        /// </summary>
        event EventHandler AfterResetHander;
        /// <summary>
        /// 异常中断游戏
        /// </summary>
        void Stoped(string message, bool clearSeatData = true, bool resetGame = true);
        /// <summary>
        /// 正常结束游戏
        /// </summary>
        void GameOver(bool clearSeatData = true, bool resetGame = true);
        /// <summary>
        /// 添加空座位
        /// </summary>
        /// <param name="n">数量(不能为0和大于游戏人数上限)</param>
        /// <returns></returns>
        ISeat AddSet(int n);
        /// <summary>
        /// 取得没有玩家的座位集合
        /// </summary>
        /// <returns></returns>
        List<ISeat> EmptySeats();
        /// <summary>
        /// 获得一个空座位
        /// </summary>
        /// <returns></returns>
        ISeat GetOneEmptySeat();
        /// <summary>
        /// 游戏开始了吗?
        /// </summary>
        /// <returns></returns>
        bool Start();
        /// <summary>
        /// 取得没人坐的位置集合
        /// </summary>
        /// <returns></returns>
        List<ISeat> NotEmptySeats();
        /// <summary>
        /// 找坐在座位上的玩家
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ISeat GetSeatByPlayerId(int id);
        /// <summary>
        /// 添加座位并坐下
        /// </summary>
        /// <param name="player">玩家</param>
        /// <returns>座位</returns>
        ISeat PlaySitDown(IPlayerJoinRoom player);
        /// <summary>
        /// 返回不是玩家的有人座位
        /// </summary>
        /// <param name="id">玩家Id</param>
        /// <returns></returns>
        List<ISeat> NotMyEmtySteats(int id);
        /// <summary>
        /// 游戏进行中检查人数够不够;
        /// </summary>
        void CheckSeatCountEnoughWhenRunning();
        /// <summary>
        /// 重置一局游戏
        /// </summary>
        void Reset(bool clearSeatData=true);
    }
}