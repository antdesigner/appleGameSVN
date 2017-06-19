using System;
using System.Collections.Generic;

namespace AntDesigner.NetCore.GameCity
{/// <summary>
/// 座位
/// </summary>
    public class Seat:ISeat
    {/// <summary>
    /// 没有人坐
    /// </summary>
        public bool IsEmpty { get;private  set; }
        /// <summary>
        /// 座位上玩家Id
        /// </summary>
        public IPlayerJoinRoom IPlayer { get; private set; }
        /// <summary>
        /// 一局游戏
        /// </summary>
        public IInningeGame IIningeGame { get;set; }
        /// <summary>
        /// 保存玩家游戏数据
        /// </summary>
        public Dictionary<string, List<string>> GameDateStr { get ; set ; } 
        /// <summary>
        /// 保存玩家游戏数据
        /// </summary>
        public Dictionary<string, List<object>> GameDateObj { get; set; }
        public Seat(IInningeGame inningeGame)
        {
            IsEmpty = true;
            GameDateStr = new Dictionary<string, List<string>>();
            GameDateObj = new Dictionary<string, List<object>>();
            IIningeGame = inningeGame;
            this.DCheckSitDown = IIningeGame.IGameProject.CheckSitDown;
            this.BeforSitDownHandler = IIningeGame.IGameProject.BeforSitDown;
            this.AfterSitDownHandler = IIningeGame.IGameProject.AfterSitDown;
            this.BeforPlayerLeaveHandler = IIningeGame.IGameProject.BeforPlayerLeave;
            this.AfterPlayerLeaveHandler = IIningeGame.IGameProject.AfterPlayerLeave;
        }
   
        /// <summary>
        /// 入座检查
        /// </summary>
        public Func<IInningeGame, bool> DCheckSitDown { get; set; }
        /// <summary>
        /// 坐下前事件
        /// </summary>
        public event EventHandler BeforSitDownHandler;
        /// <summary>
        /// 玩家坐下
        /// </summary>
        /// <param name="player_">玩家</param>
        /// <returns></returns>
        public bool PlayerSitDown(IPlayerJoinRoom player_)
        {
            if (CheckSitDown(IIningeGame))
            {
                BeforSitDownHandler?.Invoke(IIningeGame, new EventArgs());
                MyPlayerSitDown(player_);
                AfterSitDownHandler?.Invoke(IIningeGame, new EventArgs());
                return true;
            }
            return false;
        }
        /// <summary>
        /// 坐下后事件
        /// </summary>
        public event EventHandler AfterSitDownHandler;
        /// <summary>
        /// 离开座位前事件
        /// </summary>
        public event EventHandler BeforPlayerLeaveHandler;
        /// <summary>
        /// 让座位上的玩家离开
        /// </summary>
        public void PlayLeave()
        {
            BeforPlayerLeaveHandler?.Invoke(IIningeGame, new EventArgs());
            var player = IPlayer;
            IPlayer = null;
            IsEmpty = true;
            AfterPlayerLeaveHandler?.Invoke(IIningeGame, new PlayerEventArgs(player));
        }
        /// <summary>
        /// 离开座位后事件
        /// </summary>
        public event EventHandler AfterPlayerLeaveHandler;
        private  void  MyPlayerSitDown(IPlayerJoinRoom player_)
        {
          
                if (IsEmpty == true || IPlayer==null)
                {
                   IPlayer = player_;
                    IsEmpty = false;
                return;
                }
        
        }
        private bool CheckSitDown(IInningeGame inningeGame_)
        {
            if (DCheckSitDown!=null)
            {
                return DCheckSitDown(inningeGame_);
            }
            return true;
        }
        /// <summary>
        /// 清空游戏数据
        /// </summary>
  public void ClearSeatInfo()
        {
            GameDateStr.Clear();
            GameDateObj.Clear();
        }

       
    }
}