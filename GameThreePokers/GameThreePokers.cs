using AntDesigner.NetCore.GameCity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace AntDesigner.NetCore.Games.GameThreePokers {
    public class GameThreePokers : ABGameProject, IGameProject {
        #region 自定义游戏的属性区域
        /// <summary>
        /// 扑克发牌器
        /// </summary>
        PokersWithoutKingManger PokerManager { get; set; }
        /// <summary>
        /// 每次打底(入局)积分
        /// </summary>
        public decimal ChipInAmount { get; set; }
        /// <summary>
        /// 封顶积分
        /// </summary>
        public decimal LimitAmount { get; set; }
        /// <summary>
        /// 每局游戏轮次
        /// </summary>
        public int DefaultTurnCount { get; set; }
        /// <summary>
        /// 一局的起始位置
        /// </summary>
        public Seat FirstSeat { get; set; }
        /// <summary>
        /// 但前底池积分
        /// </summary>
        private decimal CurrentTotal { get; set; }
        private object LockerForCurrentTotal;
        /// <summary>
        /// 当前轮次
        /// </summary>
        private int CurrentTurn { get; set; }
        /// <summary>
        /// 加入游戏的座位
        /// </summary>
        private List<Seat> JoinSeats { get; set; }
        /// <summary>
        /// 上家是否看牌
        /// </summary>
        private bool PreSeatIsLook { get; set; }
        /// <summary>
        /// 上家下注积分
        /// </summary>
        private decimal PreSeatAmount { get; set; }
        /// <summary>
        /// 当前座位
        /// </summary>
        private Seat CurrentSeat { get; set; }
        /// <summary>
        /// 赢家座位
        /// </summary>
        private Seat WinnerSeat { get; set; }
        /// <summary>
        /// 是否结算完期间
        /// </summary>
       // private bool IsWinnerStage { get; set; }
        /// <summary>
        /// 押底关闭
        /// </summary>
      //  private bool IsChipinClose { get; set; }
        private EStage CurrentStage { get; set; }
        /// <summary>
        ///时间
        /// </summary>
        private DateTime CheckDateTime { get; set; }
        private Thread CheckThead { get; set; }
        #endregion
        public GameThreePokers() {
            #region 初始化时每项游戏必须设置的属性
            ShowName = "炸金花";//游戏名称,可根据游戏项目指定属性值
            Name = this.GetType().Name;//反射调用名称(勿修改)
            PlayerCountLimit = 5;//人数上限,可根据游戏项目指定属性值
            PlayerCountLeast = 2;//人数下限,可根据游戏项目指定属性值
            #endregion
            #region 自定义初始化区域
            LockerForCurrentTotal = new object();
            ChipInAmount = 1;
            DefaultTurnCount = 15;
            LimitAmount = 30;//封顶
            JoinSeats = new List<Seat>();
            CurrentStage = EStage.Reading;
            
            #endregion
        }
        #region 覆写ABGameProject方法区域
        public override void GameStart(object inngineGame, EventArgs e) {
            InningeGame = (IInningeGame)inngineGame;
            PokerManager = new PokersWithoutKingManger();
            if (CurrentStage == EStage.Reading) {
                CheckThead = new Thread(CheckPlayOverTime);
                CheckThead.Start();
            }
            InitPublicInfo();
            if (FirstSeat is null) {
                FirstSeat = GetRoomSeatByPlayerId(InningeGame.IRoom.RoomManager.Id);
                CurrentSeat = FirstSeat;
            }
            if (CurrentSeat is null) {
                CurrentSeat = FirstSeat;
            }
            NotifyRoomPlayers(new FreshGameFace(0));
            NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "游戏开始了!请开始押底!"));
        }
        /// <summary>
        /// 检查玩家是否超时
        /// </summary>
        /// <param name="obj"></param>
        private void CheckPlayOverTime(object obj) {
            int overSeconds=65;
            do {
                overSeconds = Math.Abs(overSeconds);
                Thread.Sleep(TimeSpan.FromSeconds(overSeconds));
                overSeconds = OverTimeSpan(overSeconds);
            } while (overSeconds< 0);
            if (InningeGame.NotEmptySeats().Count > 1&& !(CurrentSeat.IPlayer is null)) {
                int currentPlayerId = CurrentSeat.IPlayer.Id;
                do {
                    NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "有玩家超时,将被自动提出"));
                    RemoveCurrentPlayer();
                } while (currentPlayerId == CurrentSeat.IPlayer.Id);
            }
                CheckPlayOverTime(obj);
        }
        private void RemoveCurrentPlayer() {
                var room = InningeGame.IRoom;
                room.RemovePlayer(CurrentSeat.IPlayer);
        }
        public override void GameOver(object inningeGame, EventArgs e) {
            CheckDateTime = DateTime.Now;
            NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "本局结束!等待庄家开启新的一局"));
        }
        public override void ResetGame(object inningeGame, EventArgs e) {
            InitPublicInfo();
            if (FirstSeat is null) {
                FirstSeat = GetRoomSeatByPlayerId(InningeGame.IRoom.RoomManager.Id);
                CurrentSeat = FirstSeat;
            }
            if (CurrentSeat is null) {
                CurrentSeat = FirstSeat;
            }
            NotifyRoomPlayers(new FreshGameFace(0));
            NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "新一局开始!开始押底!"));
        }
        public override void Stoped(object inningeGame, EventArgs e) {
            //     if (!IsChipinClose) {
            if (CurrentStage == EStage.CanChipIning) {
                for (int i = 0; i < JoinSeats.Count(); i++) {
                    var seat = JoinSeats[0];
                    if (seat.IsChipIned) {
                        seat.IPlayer.DecutMoney(-seat.ChipinAmount);
                    }
                }
            }
            InningeGame.GameOver();
            base.Stoped(inningeGame, e);
        }
        public override bool CheckAddSeat(IInningeGame innineGame_) {
            if (innineGame_.SeatCount >= innineGame_.IGameProject.PlayerCountLimit) {
                return false;
            }
            return true;
        }
        public override void AfterSitDown(object inningeGame, EventArgs e) {
            NotifyRoomPlayers(new FreshSeatIco(InningeGame.NotEmptySeats().Count()));
            base.AfterSitDown(inningeGame, e);
            NotifyRoomPlayers(new FreshGameFace(0));
        }
        public override void BeforPlayerLeave(object inningeGame, EventArgs e) {
            IPlayerJoinRoom player = ((PlayerEventArgs)e).Player;
            Seat seat = (Seat)InningeGame.GetSeatByPlayerId(player.Id);
            ChageRole(seat);
            RemoveFromJoinSeats(seat);
            base.BeforPlayerLeave(inningeGame, e);
        }
        private void ChageRole(Seat seat) {
            bool isCurrentSeat = !IsNotCurrentSeat(seat);
            Seat nextSeat = GetNextRoomSeat(seat);
            if (IsGameRuningStage() && isCurrentSeat) {
                nextSeat = GetNextJoinSeat(seat);
                MoveToNextSeat(PreSeatAmount, PreSeatIsLook, seat.IPlayer.Id);
                if (FirstSeat == seat) {
                    FirstSeat = nextSeat;
                }
            }
            else if (isCurrentSeat) {
                nextSeat = GetNextRoomSeat(seat);
                CheckDateTime = DateTime.Now;
                CurrentSeat = nextSeat;
                if (FirstSeat == seat) {
                    FirstSeat = nextSeat;
                }

            }

        }
        private bool IsGameRuningStage() {
            if (CurrentStage == EStage.Running) {
                return true;
            }
            return false;
        }
        public override void AfterPlayerLeave(object inningeGame, EventArgs e) {
            var notEmptySeatsCount = ((IInningeGame)inningeGame).NotEmptySeats().Count();
            if (notEmptySeatsCount > 0) {
                if (CurrentStage == EStage.Running && JoinSeats.Count() == 1) {
                    CompareAll();
                    NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "有玩家要离开,人数不足,自动结算"));
                }
                else if (notEmptySeatsCount==1) {
                    CurrentStage = EStage.CanChipIning;
                    if (JoinSeats.Count()==1) {
                        var onlySeat = JoinSeats[0];
                        if (onlySeat.IsChipIned) {
                            onlySeat.IsChipIned = false;
                            IsDecutMoneySuccess(onlySeat.IPlayer, -onlySeat.ChipinAmount);
                        }
                        onlySeat.IsGaveUp = false;
                        onlySeat.IsLooked = false;
                        NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "有玩家要离开,人数不足,退还你的押底"));
                    }
                    NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "有玩家要离开,人数不足,取消押底"));
                }
                base.AfterPlayerLeave(inningeGame, e);
                NotifyRoomPlayers(new FreshGameFace(0));
            }
        }
        private void RemoveFromJoinSeats(Seat seat) {
            JoinSeats.Remove(seat);
        }
        /// <summary>
        /// 初始化每局公共信息
        /// </summary>c
        private void InitPublicInfo() {
            CurrentTotal = 0;//初始化
            CurrentTurn = 1;//初始化轮次
            JoinSeats.Clear();//次序控制
            PreSeatIsLook = false;//上家是否看牌
            PreSeatAmount = ChipInAmount;//最近下注积分
          //  IsChipinClose = false;
           // IsWinnerStage = false;
            CurrentStage = EStage.CanChipIning;
            WinnerSeat = null;
            CheckDateTime = DateTime.Now;
        }
        #endregion
        /// <summary>
        /// 刷新玩家客户端数据
        /// </summary>
        /// <param name="playerId">玩家Id</param>
        /// <returns>发送到客户端玩家数据</returns>
        /// /// <summary>
        ///  
        [CanVisitByClientAttibue]
        public object FreshGameFace(int playerId) {
           
            List<object> seats = CreatClientSeatsInfo(playerId);
            decimal myBalance =0;
            var mySeat = InningeGame.GetSeatByPlayerId(playerId);
            if (!(mySeat is null || mySeat.IPlayer is null)) {
               myBalance = mySeat.IPlayer.Account;
            }
            int winnerid = 0;
            if (!((this.WinnerSeat is null) || (WinnerSeat.IPlayer is null))) {
                winnerid = WinnerSeat.IPlayer.Id;
            }
            int joinSeatsCount = 0;
            if (!(JoinSeats is null)) {
                joinSeatsCount = this.JoinSeats.Count();
            }
            int firstId = 0;
            int currentId = 0;
            if (!(this.FirstSeat is null || FirstSeat.IPlayer is null)) {
                firstId = FirstSeat.IPlayer.Id;
            }
            if (!(this.CurrentSeat is null|| CurrentSeat.IPlayer is null )) {
                currentId = this.CurrentSeat.IPlayer.Id;
            }
            int waitSecond = 0;
            bool canCompare = false;
            if (CurrentStage == EStage.Running && GetActiveSeatCount() == 2) {
                canCompare = true;
            }
            waitSecond = (DateTime.Now - CheckDateTime).Seconds;
            var PublicInfo = new {
                MyId = playerId,
                /// 每次打底(入局)积分
                ChipInAmount = this.ChipInAmount,
                /// 封顶积分
                LimitAmount = this.LimitAmount,
                /// 每局游戏轮次
                DefaultTurnCount = this.DefaultTurnCount,
                /// 一局的起始位置
                FirstSeatPlayerId = firstId,
                /// 但前底池积分
                CurrentTotal = this.CurrentTotal,
                /// 当前轮次
                CurrentTurn = this.CurrentTurn,
                /// 上家是否看牌
                PreSeatIsLook = this.PreSeatAmount,
                /// 上家下注积分
                PreSeatAmount = this.PreSeatAmount,
                /// 当前座位
                CurrentSeatPlayId = currentId,
                /// 赢家座位
                WinnerSeatPlayerId = winnerid,
                ///是否结算后期间
              //  IsWinnerStage = this.IsWinnerStage,
                ///打底关闭
            //    IsChipinClose = this.IsChipinClose,

                //可以比牌
                CanCompare = canCompare,
                //阶段
                CurrentStage = this.CurrentStage,
                ///房价玩家总数
                playerCount = this.InningeGame.NotEmptySeats().Count(),
                ///已打底数
                playerChipinCount = joinSeatsCount,
                ///等待玩家操作秒数
                WaitSecond = waitSecond,
                //玩家余额
                Balance = myBalance
            };
            var GameFace = new {
                PublicInfo = PublicInfo,
                Seats = seats
            };
            return GameFace;
        }
        /// <summary>
        /// 构造发送给客户端刷新界面的座位信息对象
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private List<object> CreatClientSeatsInfo(int playerId) {
            List<object> seats = new List<object>();
            int n = 0;
            lock (JoinSeats) {
                Seat mySeat = GetJionSeatByPlayerId(playerId);
                List<Seat> tempSeats = new List<Seat>();
                TakeMyseatToLast(mySeat, tempSeats);
                for (int i = 0; i < tempSeats.Count; i++) {
                    var seat = tempSeats[i];
                    var PokersShow = new List<Card>();
                    for (int k = 0; k < 3; k++) {
                        PokersShow.Add(new Card(0, "", ""));
                    }
                    int j = n++;
                    if (seat.IPlayer.Id == playerId) {
                        j = 4;
                        PokersShow = seat.PokersShow;
                    }
                    // if (IsWinnerStage && (!seat.IsGaveUp)) {
                    if (CurrentStage==EStage.Computed && (!seat.IsGaveUp)) {
                        PokersShow = seat.Pokers.CardsSort;
                    }
                    var PokerOtherCanSee = new Card(0, "", "");
                    if (!(mySeat is null) && seat.IPlayer.Id == mySeat.PlayerIdWhichCanSee) {
                        PokerOtherCanSee = seat.PokerOtherCanSee;
                      //  seat.PokersShow[2] = PokerOtherCanSee;
                    }
                    var seatInfo = new {
                        canvId = j,
                        playerId = seat.IPlayer.Id,
                        /// 可以看的牌
                        PokersShow = PokersShow,
                        ///别人可以看的牌Q
                        PokerOtherCanSee = PokerOtherCanSee,
                        /// 可以查看其他玩家的一张牌的Id
                        PlayerIdWhichCanSee = seat.PlayerIdWhichCanSee,
                        /// 自己上一次下注积分
                        PreChipInAmount = seat.PreChipInAmount,
                        /// 是否已押底
                        IsChipIned = seat.IsChipIned,
                        /// 是否已看牌
                        IsLooked = seat.IsLooked,
                        /// 是否已弃牌
                        IsGaveUp = seat.IsGaveUp,
                        ///下注方式
                        ChipinType = seat.PreChipType
                    };
                    seats.Add(seatInfo);
                }
            }
            return seats;
        }
        private void TakeMyseatToLast(Seat mySeat, List<Seat> tempSeats) {
            for (int i = 0; i < JoinSeats.Count; i++) {
                var seat = JoinSeats[i];
                if ((mySeat is null) || (seat.IPlayer.Id != mySeat.IPlayer.Id)) {
                    tempSeats.Add(seat);
                }
            }
            if (mySeat is null) {
                return;
            }
            tempSeats.Add(mySeat);
        }
        /// <summary>
        ///创建游戏项目的特性座位对象,继承自seat
        /// </summary>
        /// <param name="inningeGame"></param>
        /// <returns></returns>
        public ISeat CreatSeat(IInningeGame inningeGame) {
            return new Seat(inningeGame);
        }
        #region 自定义方法区域
        /// <summary>
        /// 发牌
        /// </summary>
        [CanVisitByClientAttibue]
        public void PlayerChipin(int playerId) {
            //  if (IsChipinClose) {
            if (CurrentStage != EStage.CanChipIning && CurrentStage != EStage.Computed) {
                NotifySinglePlayer(WebscoketSendObjs.RoomMessage(0, "未开局或结束,不能押底!"), playerId);
                return;
            }
            Seat seat = GetRoomSeatByPlayerId(playerId);
            if (seat.IsChipIned) {
                NotifySinglePlayer(WebscoketSendObjs.RoomMessage(0, "已经押底!"), playerId);
                NotifyRoomPlayers(new FreshGameFace(0));
                return;
            }
            if (InningeGame.NotEmptySeats().Count < 2) {
                NotifySinglePlayer(WebscoketSendObjs.RoomMessage(0, "房间人数少于两人,不能押底"), playerId);
                NotifyRoomPlayers(new FreshGameFace(0));
                return;
            }
            lock (seat) {
                if (seat.PlayerChipin(ChipInAmount, IsDecutMoneySuccess)) {
                    AddPlaySeat(seat);
                    AddCurrentTotal(ChipInAmount);
                }
            }
            CheckDateTime = DateTime.Now;
            NotifyRoomPlayers(new FreshGameFace(0));
            NotifySinglePlayer(WebscoketSendObjs.RoomMessage(0, "已押底,等待庄家发牌!"), playerId);
        }
        /// <summary>
        /// 发牌
        /// </summary>
        /// <param name="playerId"></param>
        [CanVisitByClientAttibue]
        public void Deal(int playerId) {
            // if (IsChipinClose) {
            if (CurrentStage != EStage.CanChipIning) {
                NotifySinglePlayer(WebscoketSendObjs.RoomMessage(playerId, "不是押底阶段,不能发牌!"), playerId);
                return;
            }
            // if (JoinSeats.Count() != InningeGame.NotEmptySeats().Count()) {
            if (JoinSeats.Count() < 2) {
                NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "押底人数不足2人,不能发牌"));
                return;
            }
            //  IsChipinClose = true;//?
            PokerManager.Riffile();
            lock (this) {
                for (int i = 0; i < JoinSeats.Count(); i++) {
                    ThreeCards threeCards = new ThreeCards(PokerManager.TackOut(3));
                    JoinSeats[i].Pokers = threeCards;
                    JoinSeats[i].PreChipType = EChipinType.PlayerChipIn;
                }
                CurrentStage = EStage.Running;
            }
            CheckDateTime = DateTime.Now;
            NotifyRoomPlayers(new FreshGameFace(0));
            NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "发牌完毕,庄家开始下注"));
        }
        /// <summary>
        /// 玩家看牌一张
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [CanVisitByClientAttibue]
        public void Look(int playerId) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (seat is null) {
                NotifySinglePlayer(WebscoketSendObjs.RoomMessage(playerId, "没有加入本局,自己无牌!"), playerId);
                return;
            }
            if (seat.Pokers is null || seat.Pokers.Cards.Count != 3) {
                NotifySinglePlayer(WebscoketSendObjs.RoomMessage(playerId, "还未发牌,不能查看!"), playerId);
                return;
            }
            seat.LookOneCard();
            NotifySinglePlayer(new FreshGameFace(playerId), playerId);
        }
        /// <summary>
        /// 玩家天眼查看其他玩家一张牌
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="otherPlayerId"></param>
        /// <returns></returns>
        [CanVisitByClientAttibue]
        public object LookOthersPoker(int playerId, int otherPlayerId) {
            var Poker = new Poker("", "", 0);
            if (CurrentStage!=EStage.Running) {
                return Poker;
            }
            Seat mySeat = GetJionSeatByPlayerId(playerId);
            Seat otherSeat = GetJionSeatByPlayerId(otherPlayerId);
           
            if (mySeat.PlayerIdWhichCanSee != 0) {
                NotifySinglePlayer(new Alert(playerId, "已经查看过一个玩家,不能再查看"), playerId);
                return Poker;
            }
            if (IsDecutMoneySuccess(mySeat.IPlayer, 1)) {
                Card card = otherSeat.GetOtherCanSeePoker();
                Poker = new Poker(card.CardColor, card.Name, card.ComparedValue);
                mySeat.PlayerIdWhichCanSee = otherPlayerId;
                return Poker;
            }
            return Poker;
        }
        /// <summary>
        /// 玩家表态_暗注
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="amount"></param>
        [CanVisitByClientAttibue]
        public void ChipInNoLook(int playerId, decimal amount) {
            if (IsAmountLessThanPre(amount)) {
                return;
            }
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (IsNotCurrentSeat(seat)) {
                return;
            }
            if (seat.ChipIn(amount, IsDecutMoneySuccess, EChipinType.NoLook)) {
                MoveToNextSeat(amount, false, playerId, EChipinType.NoLook);
            }
        }
        /// <summary>
        /// 玩家表态_跟
        /// </summary>
        /// <param name="playerId"></param>
        [CanVisitByClientAttibue]
        public void ChipInFollow(int playerId) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (IsNotPassedCheck(seat)) {
                return;
            }
            bool iHadLook = seat.IsLooked;
            decimal chipInAmount = PreSeatAmount;
            if (iHadLook != PreSeatIsLook) {
                if (PreSeatIsLook) {
                    chipInAmount = chipInAmount / 2;
                }
                else {
                    chipInAmount = chipInAmount * 2;
                }
            }
            if (chipInAmount > LimitAmount) {
                chipInAmount = LimitAmount;
            }
            //if (IsDecutMoneySuccess(seat.IPlayer, chipInAmount)) {
            //    MoveToNextSeat(chipInAmount, iHadLook,playerId, EChipinType.Follow);
            //}
            if (seat.ChipIn(chipInAmount, IsDecutMoneySuccess, EChipinType.Follow)) {
                MoveToNextSeat(chipInAmount, iHadLook, playerId, EChipinType.Follow);
            }
        }
        /// <summary>
        ///  玩家放弃
        /// </summary>
        /// <param name="playerId"></param>
        [CanVisitByClientAttibue]
        public void Giveup(int playerId) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (IsNotPassedCheck(seat)) {
                return;
            }
            // seat.IsGaveUp = true;
            seat.Giveup();
            if (FirstSeat == seat) {
                FirstSeat = GetNextJoinSeat(seat);
            }
            MoveToNextSeat(PreSeatAmount, PreSeatIsLook, playerId,EChipinType.GaveUp);
        }
        /// <summary>
        /// 玩家表态_直封
        /// </summary>
        /// <param name="playerId"></param>
        [CanVisitByClientAttibue]
        public void ChipInLimit(int playerId) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (IsNotPassedCheck(seat)) {
                return;
            }
            bool iHadLook = seat.IsLooked;
            decimal chipInAmount = PreSeatAmount;
            if (iHadLook) {
                chipInAmount = LimitAmount;
            }
            else {
                chipInAmount = LimitAmount / 2;
            }
            //if (IsDecutMoneySuccess(seat.IPlayer, chipInAmount)) {
            //    MoveToNextSeat(chipInAmount, iHadLook,playerId, EChipinType.Limit);
            //}
            if (seat.ChipIn(chipInAmount, IsDecutMoneySuccess, EChipinType.Limit)) {
                MoveToNextSeat(chipInAmount, iHadLook, playerId, EChipinType.Limit);
            }
        }
        /// <summary>
        /// 玩家表态_加倍
        /// </summary>
        /// <param name="playerId"></param>
        [CanVisitByClientAttibue]
        public void ChipInDouble(int playerId) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (IsNotPassedCheck(seat)) {
                return;
            }
            bool iHadLook = seat.IsLooked;
            decimal chipInAmount = 2 * PreSeatAmount;
            if (iHadLook != PreSeatIsLook) {
                if (PreSeatIsLook) {
                    chipInAmount = chipInAmount / 2;
                }
                else {
                    chipInAmount = chipInAmount * 2;
                }
            }
            if (chipInAmount > LimitAmount) {
                chipInAmount = LimitAmount;
            }
            //if (IsDecutMoneySuccess(seat.IPlayer, chipInAmount)) {
            //    MoveToNextSeat(chipInAmount, iHadLook,playerId, EChipinType.Double);
            //}
            if (seat.ChipIn(chipInAmount, IsDecutMoneySuccess, EChipinType.Double)) {
                MoveToNextSeat(chipInAmount, iHadLook, playerId, EChipinType.Double);
            }
        }
        /// <summary>
        /// 玩家表态_自定义积分
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="amount"></param>
        [CanVisitByClientAttibue]
        public object ChipIn(int playerId, decimal amount) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (IsNotPassedCheck(seat)) {
                return new {
                    rl = ""
                };
            }
            bool iHadLook = seat.IsLooked;
            if (PreSeatIsLook && !iHadLook) {
                if (IsAmountLessThanPre(amount * 2)) {
                    return new {
                        rl = "金额有误,不能低于上家金额"
                    };
                }
            }
            if (!PreSeatIsLook && iHadLook) {
                if (IsAmountLessThanPre(amount / 2)) {
                    return new {
                        rl = "上家是暗注,你金额有误"
                    };
                }
            }
            if (IsAmountLessThanPre(amount)) {
                return new {
                    rl = "金额有误,不能低于上家金额"
                };
            }
            if (amount > LimitAmount) {
                amount = LimitAmount;
            }
            //if (IsDecutMoneySuccess(seat.IPlayer, amount)) {
            //    MoveToNextSeat(amount, iHadLook,playerId, EChipinType.ChipIn);
            //}
            if (seat.ChipIn(amount, IsDecutMoneySuccess, EChipinType.ChipIn)) {
                MoveToNextSeat(amount, iHadLook, playerId, EChipinType.ChipIn);
            }
            return new {
                rl = 1
            };
        }
        /// <summary>
        ///开牌
        /// </summary>
        /// <param name="playerId"></param>
        [CanVisitByClientAttibue]
        public void Compare(int playerId) {
            if (CurrentStage != EStage.Running) {
                return;
            }
            Seat seat = (Seat)GetJionSeatByPlayerId(playerId);
            if (IsNotPassedCheck(seat)) {
                return;
            }
            bool iHadLook = seat.IsLooked;
            Seat opponetSeat = GetOnlyOpponentSeat();
            if (null == opponetSeat) {
                return;
            }
            CheckDateTime = DateTime.Now;
            decimal chipInAmount = PreSeatAmount;
            if (iHadLook != PreSeatIsLook) {
                if (PreSeatIsLook) {
                    chipInAmount = chipInAmount / 2;
                }
                else {
                    chipInAmount = chipInAmount * 2;
                }
            }
            if (chipInAmount > LimitAmount) {
                chipInAmount = LimitAmount;
            }
            if (!IsDecutMoneySuccess(seat.IPlayer, chipInAmount)) {
                return;
            }
            AddCurrentTotal(chipInAmount);
            seat.PreChipInAmount = chipInAmount;
            seat.PreChipType = EChipinType.Compare;
            WinnerSeat = GetWinner(seat, opponetSeat);
            decimal playerWinAmount = 0;
            playerWinAmount = SystemTax(playerWinAmount);
            WinnerSeat.IPlayer.DecutMoney(-playerWinAmount);
            // IsWinnerStage = true;//?
            FirstSeat = WinnerSeat;//新庄家
            CurrentSeat = WinnerSeat;
            CurrentStage = EStage.Computed;
            NotifyRoomPlayers(new ChipinAnimation(0, chipInAmount, playerId, EChipinType.Compare));
            NotifyRoomPlayers(new FreshGameFace(0));
            InningeGame.GameOver(false, false);
        }
        /// <summary>
        /// 满足最大轮次等条件时自动开牌
        /// </summary>
        private void CompareAll() {
            if (CurrentStage != EStage.Running) {
                return;
            }
            CheckDateTime = DateTime.Now;
            List<Seat> notGaveupSeats = new List<Seat>();
            if (JoinSeats.Count != 0) {
                CheckDateTime = DateTime.Now;
                for (int i = 0; i < JoinSeats.Count; i++) {
                    if (JoinSeats[i].IsGaveUp) {
                        continue;
                    }
                    notGaveupSeats.Add(JoinSeats[i]);
                }
                WinnerSeat = (notGaveupSeats.OrderByDescending(s => s.Pokers).ToList())[0];
            }
            decimal playerWinAmount = 0;
            playerWinAmount = SystemTax(playerWinAmount);
            WinnerSeat.IPlayer.DecutMoney(playerWinAmount);
            FirstSeat = WinnerSeat;//新庄家
            CurrentSeat = WinnerSeat;
            CurrentStage = EStage.Computed;
            NotifyRoomPlayers(new FreshGameFace(0));
            InningeGame.GameOver(false, false);//触发游戏结束事件
        }
        private decimal SystemTax(decimal playerWinAmount) {
            if (CurrentTotal > 10) {
                playerWinAmount = CurrentTotal - 1;
            }
            return playerWinAmount;
        }
        /// <summary>
        /// 根据玩家Id获得对应座位
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private Seat GetJionSeatByPlayerId(int playerId) {

            return JoinSeats.Find(s => s.IPlayer.Id == playerId);
        }
        /// <summary>
        /// 更据Id获得在房间的玩家座位
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private Seat GetRoomSeatByPlayerId(int playerId) {
            return (Seat)InningeGame.GetSeatByPlayerId(playerId);
        }
        /// <summary>
        /// 玩家押底
        /// </summary>
        /// <param name="playerId"></param>
        public int OverTimeSpan(int seconds) {
            var currentTime = DateTime.Now;
            TimeSpan ts = currentTime.Subtract(CheckDateTime);
            return ts.Seconds - seconds;
        }
        /// <summary>
        /// 增加底池
        /// </summary>
        /// <param name="amount"></param>
        private void AddCurrentTotal(decimal amount) {
            lock (LockerForCurrentTotal) {
                CurrentTotal = CurrentTotal + amount;//增加锅底金额
            }
        }
        private bool IsAmountLessThanPre(decimal amount) {
            if (amount < PreSeatAmount) {
                return true;
            }
            return false;
        }
       /// <summary>
       /// 
       /// </summary>
        public void SettolAccount() {
        }
        /// <summary>
        /// 玩家押底入局
        /// </summary>
        /// <param name="seat"></param>
        private void AddPlaySeat(Seat seat) {
            if (JoinSeats.Exists(s => s.IPlayer.Id == seat.IPlayer.Id)) {
                return;
            }
            JoinSeats.Add(seat);
        }
        /// <summary>
        /// 扣款成功
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private bool IsDecutMoneySuccess(IPlayerJoinRoom player, decimal amount) {
            if (IsAccountEnougth(player, amount)) {
                player.DecutMoney(amount);
                return true;
            }
            NotifySinglePlayer(new Alert(player.Id, "账户余额不足"), player.Id);
            return false;
        }
        /// <summary>
        /// 账户是否够
        /// </summary>
        /// <param name="player"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private bool IsAccountEnougth(IPlayerJoinRoom player, decimal amount) {

            if (player.AccountNotEnough(amount)) {
                NotifySinglePlayer(WebscoketSendObjs.Alert(player.Id, "账户余额不足"), player.Id);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获得座位得到的牌
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        private ThreeCards GetSeatGetPokers(Seat seat) {
            if (!(seat.Pokers is null)) {
                return seat.Pokers;
            }
            return null;
        }
        /// <summary>
        /// 获得两个牌大的玩家座位
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="opponetSeat"></param>
        /// <returns></returns>
        private Seat GetWinner(Seat seat, Seat opponetSeat) {
            ThreeCards myCards = GetSeatGetPokers(seat);
            ThreeCards opponentCards = GetSeatGetPokers(opponetSeat);
            if (myCards > opponentCards) {
                return seat;
            }
            else {
                return opponetSeat;
            }
        }
        /// <summary>
        /// 获得被开牌方座位
        /// </summary>
        /// <returns></returns>
        private Seat GetOnlyOpponentSeat() {
            List<Seat> seats = JoinSeats.FindAll(s => s.IPlayer.Id != CurrentSeat.IPlayer.Id);
            List<Seat> notGaveupSeats = new List<Seat>();
            for (int i = 0; i < seats.Count; i++) {
                if (seats[i].IsGaveUp) {
                    continue;
                }
                notGaveupSeats.Add(seats[i]);
            }
            if (notGaveupSeats.Count != 1) {
                return null;
            }
            return seats[0];
        }
        /// <summary>
        /// 获得下一个没有放弃的座位
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        private Seat GetNextJoinSeat(Seat seat) {
            for (int i = 0; i < JoinSeats.Count; i++) {
                if (JoinSeats[i] == seat) {
                    if (i == JoinSeats.Count - 1) {
                        return JoinSeats[0];
                    }
                    else {
                        return JoinSeats[i + 1];
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获得房间下一个有玩家的座位
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        private Seat GetNextRoomSeat(Seat seat) {
            var notEmptySeats = InningeGame.NotEmptySeats();
            for (int i = 0; i < notEmptySeats.Count; i++) {
                if (notEmptySeats[i] == seat) {
                    if (i == notEmptySeats.Count - 1) {
                        return(Seat)notEmptySeats[0];
                    }
                    else {
                        return (Seat)notEmptySeats[i + 1];
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 轮到下一个玩家表态
        /// </summary>
        private void MoveToNextSeat(decimal amount, bool isLook, int playerId, EChipinType chipinType = EChipinType.Nothing) {
            lock (this) {
                PreSeatAmount = amount;
                if (chipinType!=EChipinType.GaveUp) {
                    CurrentSeat.PreChipInAmount = amount;
                    CurrentSeat.PreChipType = chipinType;
                    AddCurrentTotal(amount);
                }
                Seat tempSeat = CurrentSeat;
                do {
                    tempSeat = GetNextJoinSeat(tempSeat);
                } while (tempSeat.IsGaveUp);
                CurrentSeat = tempSeat;
                PreSeatIsLook = isLook;
                if (CurrentSeat == FirstSeat) {
                    CurrentTurn++;
                }
                if (CurrentTurn > DefaultTurnCount) {
                    NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "超过最大轮次,自动比牌"));
                    CompareAll();
                }
                if (JoinSeats.Where(s => s.IsGaveUp == false).Count() == 1) {
                    CompareAll();
                }
            }
            CheckDateTime = DateTime.Now;
            NotifyRoomPlayers(new ChipinAnimation(0, amount, playerId, chipinType));
            NotifyRoomPlayers(new FreshGameFace(0));
        }
        /// <summary>
        /// 判断是否轮到
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        private bool IsNotCurrentSeat(Seat seat) {
            if (CurrentSeat == seat && CurrentSeat.IPlayer.Id == seat.IPlayer.Id) {
                return false;
            }
            else {
                return true;
            }

        }
        /// <summary>
        /// 检测方法调用资格
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        private bool IsNotPassedCheck(Seat seat) {
            if (seat is null) {
                return true;
            }
            if (seat.Pokers is null) {
                return true;
            }
            if (IsNotCurrentSeat(seat)) {
                return true;
            }
            if (!seat.IsChipIned || seat.IsGaveUp) {
                return true;
            }
            //if (IsWinnerStage) {
            //    return true;
            //}
            if (CurrentStage==EStage.Computed) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 提供给其他玩家天眼查的一张牌
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        private Card GetOtherCanSeePoker(Seat seat) {
            if (null != seat.PokerOtherCanSee) {
                return seat.PokerOtherCanSee;
            }
            seat.PokerOtherCanSee = seat.Pokers.Cards[0];
            return seat.PokerOtherCanSee;
        }
        /// <summary>
        /// 返回每局参与玩家
        /// </summary>
        /// <returns></returns>
        private List<Seat> GetPlaySeats() {
            return JoinSeats.FindAll(p => p.IsChipIned == true);
        }
        /// <summary>
        /// 没有放弃的玩家数量
        /// </summary>
        private int GetActiveSeatCount() {
            int count = JoinSeats.FindAll(s => s.IsGaveUp == false).Count();
            return count;
        }
        #endregion
    }

}
