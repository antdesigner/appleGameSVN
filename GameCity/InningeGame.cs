using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntDesigner.NetCore.GameCity {/// <summary>
/// 一局游戏
/// </summary>
    public class InningeGame : IInningeGame {
        /// <summary>
        /// 添加座位后事件
        /// </summary>
        public event EventHandler AfterAddSeatHandler;
        /// <summary>
        /// 即将添加座位前事件
        /// </summary>
        public event EventHandler BeforAddSeatHandler;
        /// <summary>
        /// 可以启动游戏前事件
        /// </summary>
        public event EventHandler BeforGameStartHandler;
        /// <summary>
        /// 启动了游戏事件
        /// </summary>
        public event EventHandler GameStartHandler;
        /// <summary>
        /// 游戏异常中断事件
        /// </summary>
        public event EventHandler StoptedHandler;
        /// <summary>
        /// 游戏正常结束事件
        /// </summary>
        public event EventHandler GameOverHander;
        /// <summary>
        ///重置游戏后事件
        /// </summary>
        public event EventHandler AfterResetHander;

        /// <summary>
        /// 不能直接添加元素
        /// </summary>
        List<ISeat> Seats { get; set; }
        /// <summary>
        /// 游戏开始了?
        /// </summary>
        public bool IsStarted { get; private set; }
        /// <summary>
        /// 游戏(不能通过属性更换)
        /// </summary>
        public IGameProject IGameProject { get; set; }
        /// <summary>
        /// 这局游戏的座位数
        /// </summary>
        public int SeatCount { get { return Seats.Count; } private set { } }
        /// <summary>
        /// 委托检查是否可以启动游戏
        /// </summary>
        public Func<IInningeGame, bool> DCheckStart { get; set; }
        /// <summary>
        /// 委托检查能否添加座位
        /// </summary>
        public Func<IInningeGame, bool> DCheckAddSeat { get; set; }
        public Func<IInningeGame, ISeat> DCreatSeat { get; set; }
        //public Dictionary<string, List<string>> GameDataStr { get; set; }
        //public Dictionary<string, List<object>> GameDataObj { get; set; }
        //public Dictionary<string, List<int>> GameDataInt { get; set; }
        //public Dictionary<string, List<bool>> GameDataBool { get; set; }
        //public Dictionary<string, List<decimal>> GameDataDecimal { get; set; }
        public IRoom IRoom { get; set; }
        /// <summary>
        /// 异常中断标记
        /// </summary>
        public bool IsStoped { get; private set; }
        /// <summary>
        /// 正常结束标记
        /// </summary>
        public bool IsGameOver { get; private set; }

        /// <summary>
        /// 一般不用
        /// </summary>
        public InningeGame() {
            IsStarted = false;
            IsStoped = false;
            IsGameOver = false;
            Seats = new List<ISeat>();
        }
        public InningeGame(IGameProject gameProject_) : this() {
            IGameProject = gameProject_;
            DCheckAddSeat += IGameProject.CheckAddSeat;
            DCheckStart += IGameProject.CheckStart;
            GameStartHandler += IGameProject.GameStart;
            BeforAddSeatHandler += IGameProject.BeforAddSeat;
            AfterAddSeatHandler += IGameProject.AfterAddSeat;
            BeforGameStartHandler += IGameProject.BeforGameStart;
            StoptedHandler += IGameProject.Stoped;
            GameOverHander += IGameProject.GameOver;
            AfterResetHander += IGameProject.ResetGame;
        }
        /// <summary>
        /// 游戏所在房间导航
        /// </summary>
        //public IRoom Room { get; set; }
        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <returns></returns>
        public bool Start() {
            if (CheckStart()) {
                BeforGameStartHandler?.Invoke(this, new EventArgs());
                if (MyStart()) {
                    GameStartHandler?.Invoke(this, new EventArgs());
                    return true;
                }
            }
            return false;
        }
        private bool CheckStart() {
            if (IsStarted == true || IsStoped == true || IsGameOver == true) {
                return false;
            }
            if (DCheckStart != null) {
                return DCheckStart(this);
            }
            return true;
        }
        private bool MyStart() {
            if (SeatCount == 0) {
                throw new Exception("座位数量不能为零");
            }
            if (IGameProject.PlayerCountLeast > SeatCount || IGameProject.PlayerCountLimit < SeatCount) {
                IsStarted = false;
                return false;
            }
            IsStarted = true;
            return IsStarted;
        }
        /// <summary>
        /// 添加座位
        /// </summary>
        /// <param name="n"></param>
        public ISeat AddSet(int n) {
            if (CheckAddSeat()) {
                BeforAddSeatHandler?.Invoke(this, new EventArgs());
                MyAddSet(n);
                AfterAddSeatHandler?.Invoke(this, new EventArgs());
                //  return true;
            }
            return GetOneEmptySeat();
        }
        /// <summary>
        /// 能否添加座位检查
        /// </summary>
        /// <returns></returns>
        private bool CheckAddSeat() {
            if (DCheckAddSeat != null) {
                return DCheckAddSeat(this);
            }
            return true;
        }
        private void MyAddSet(int n) {
            for (int i = 0; i < n; i++) {
                if (IGameProject.PlayerCountLimit == SeatCount) {
                    break;
                   // throw new Exception("座位已经达到游戏上限,不能再添加");
                }
                ISeat seat = null;
                if (DCreatSeat != null) {
                    seat = DCreatSeat(this);
                }
                Seats.Add(seat);
            }
        }
        /// <summary>
        /// 获得全部没有玩家的座位
        /// </summary>
        /// <returns></returns>
        public List<ISeat> EmptySeats() {
            return Seats.Where<ISeat>(s => s.IsEmpty == true).ToList();
        }
        /// <summary>
        /// 获得一个空座位
        /// </summary>
        /// <returns></returns>
        public ISeat GetOneEmptySeat() {
            List<ISeat> emptySeats = EmptySeats();
            if (emptySeats.Count == 0) {
                return null;
            }
            return emptySeats[0];
        }
        /// <summary>
        /// 有玩家的座位集合
        /// </summary>
        /// <returns></returns>
        public List<ISeat> NotEmptySeats() {
            return Seats.Where<ISeat>(s => s.IsEmpty == false).ToList();
        }
        /// <summary>
        /// 返回不是我的但有人的座位
        /// </summary>
        /// <param name="id">玩家Id</param>
        /// <returns></returns>
        public List<ISeat> NotMyEmtySteats(int id) {
            return Seats.Where<ISeat>(s => s.IsEmpty == false && s.IPlayer.Id != id).ToList();
        }
        /// <summary>
        ///依据玩家Id返回座位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ISeat GetSeatByPlayerId(int id) {
            return Seats.FirstOrDefault(s => s.IPlayer != null && s.IPlayer.Id == id);
        }
        /// <summary>
        /// 添加座位坐下
        /// </summary>
        /// <param name="player">玩家</param>
        /// <returns>座位</returns>
        public ISeat PlaySitDown(IPlayerJoinRoom player) {
            ISeat seat = GetOneEmptySeat();
            if (seat == null) {
                seat = AddSet(1);
            }
            seat.PlayerSitDown(player);
            return seat;
        }
        /// <summary>
        /// 异常中断游戏
        /// </summary>
        public void Stoped(string message, bool clearSeatData = true, bool resetGame = true) {
            IsStoped = true;
            StoptedHandler?.Invoke(this, new GameStopedEventArgs(message));
            if (resetGame) {
                Reset(clearSeatData);
            }

        }
        /// <summary>
        /// 正常结束游戏
        /// </summary>
        public void GameOver(bool clearSeatData=true, bool resetGame = true) {
            IsGameOver = true;
            if (resetGame) {
                Reset(clearSeatData);
            }
            GameOverHander?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// 游戏进行人数够不够
        /// </summary>
        public void CheckSeatCountEnoughWhenRunning() {
            if (IsStarted == true) {
                if (NotEmptySeats().Count < IGameProject.PlayerCountLeast) {
                    Stoped("有玩家离开,游戏人数不足");
                }
            }

        }
        /// <summary>
        /// 重置游戏
        /// </summary>
        public void Reset(bool clearSeatData=true) {
            if (clearSeatData) {
                ClearAllSeatInfo();
            }
            IsStarted = true;
            IsStoped = false;
            IsGameOver = false;
            AfterResetHander?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// 清除本局全部座位游戏数据
        /// </summary>
        private void ClearAllSeatInfo() {
            for (int i = 0; i < Seats.Count(); i++) {
                Seats[i].ClearSeatInfo();
            }
        }
    }
}
