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
        private bool IsWinnerStage { get; set; }
        /// <summary>
        /// 押底关闭
        /// </summary>
        private bool IsChipinClose { get; set; }
        /// <summary>
        ///时间
        /// </summary>
        private DateTime CheckDateTime { get; set; }
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
            DefaultTurnCount = 3;
            LimitAmount = 50;//封顶
            JoinSeats = new List<Seat>();
            #endregion
        }
        #region 覆写ABGameProject方法区域
        public override void GameStart(object inngineGame, EventArgs e) {
            InningeGame = (IInningeGame)inngineGame;
            PokerManager = new PokersWithoutKingManger();
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
            if (!IsChipinClose) {
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
            Seat nextSeat = GetNextSeat(seat);
            if (IsGameRuningStage()&&isCurrentSeat){
                MoveToNextSeat(PreSeatAmount, PreSeatIsLook, seat.IPlayer.Id);
            } else if (isCurrentSeat) {
                CheckDateTime = DateTime.Now;
                CurrentSeat = nextSeat;

            }
            if (FirstSeat == seat) {
                FirstSeat = nextSeat;
            }
        }
        private  bool IsGameRuningStage() {
            if (IsChipinClose && (!IsWinnerStage)) {
                return true;
            }
            return false;
        }
        public override void AfterPlayerLeave(object inningeGame, EventArgs e) {

            // if (IsChipinClose&& !(WinnerSeat is null)&&JoinSeats.Count()==1) {
            if (IsChipinClose && (!IsWinnerStage)&& JoinSeats.Count() == 1) {
                CompareAll();
            }
            base.AfterPlayerLeave(inningeGame, e);
            NotifyRoomPlayers(new FreshGameFace(0));
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
            IsChipinClose = false;
            IsWinnerStage = false;
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
        public object FreshGameFace(int playerId) {
            List<object> seats = CreatClientSeatsInfo(playerId);
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
            if (!(this.FirstSeat is null)) {
                firstId = FirstSeat.IPlayer.Id;
            }
            if (!(this.CurrentSeat is null)) {
                currentId = this.CurrentSeat.IPlayer.Id;
            }
            int waitSecond = 0;
            waitSecond = (DateTime.Now - CheckDateTime).Seconds;
            var PublicInfo = new {
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
                IsWinnerStage = this.IsWinnerStage,
                ///打底关闭
                IsChipinClose = this.IsChipinClose,
                ///房价玩家总数
                playerCount = this.InningeGame.NotEmptySeats().Count(),
                ///已打底数
                playerChipinCount = joinSeatsCount,
                ///等待玩家操作秒数
               WaitSecond = waitSecond
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
                    if (IsWinnerStage && (!seat.IsGaveUp)) {
                        PokersShow = seat.Pokers.CardsSort;
                    }
                    var PokerOtherCanSee = new Card(0, "", "");
                    if (!(mySeat is null) && seat.IPlayer.Id == mySeat.PlayerIdWhichCanSee) {
                        PokerOtherCanSee = seat.PokerOtherCanSee;
                        seat.PokersShow[2] = PokerOtherCanSee;
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
                if ((mySeat is null )||( seat.IPlayer.Id != mySeat.IPlayer.Id) ){
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
        /// <summary>
        /// 根据玩家Id获得对应座位
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        private Seat GetJionSeatByPlayerId(int playerId) {

            return JoinSeats.Find(s => s.IPlayer.Id == playerId);
        }
        private Seat GetRoomSeatByPlayerId(int playerId) {
            return (Seat)InningeGame.GetSeatByPlayerId(playerId);
        }
        #region 自定义方法区域
        /// <summary>
        /// 玩家押底
        /// </summary>
        /// <param name="playerId"></param>
        public void PlayerChipin(int playerId) {
            if (IsChipinClose) {
                NotifySinglePlayer(WebscoketSendObjs.RoomMessage(playerId, "游戏已正式开始,不能押底!"), playerId);
                return;
            }
            Seat seat = GetRoomSeatByPlayerId(playerId);
            lock (seat) {
                //if (seat.IsChipIned) {
                //    return;
                //}
                //if (IsDecutMoneySuccess(seat.IPlayer, ChipInAmount)) {
                //    AddPlaySeat(seat);
                //    seat.IsChipIned = true;
                //    seat.ChipinAmount = ChipInAmount;
                if (seat.PlayerChipin(ChipInAmount, IsDecutMoneySuccess)) {
                    AddPlaySeat(seat);
                    AddCurrentTotal();
                }
            }
            CheckDateTime = DateTime.Now;
            NotifyRoomPlayers(new FreshGameFace(0));
            NotifySinglePlayer(WebscoketSendObjs.RoomMessage(playerId, "已押底,等待庄家发牌!"), playerId);
        }
        private void AddCurrentTotal() {
            lock (LockerForCurrentTotal) {
                CurrentTotal = CurrentTotal + ChipInAmount;//增加锅底金额
            }
        }
        /// <summary>
        /// 发牌
        /// </summary>
        public void Deal() {
            if (IsChipinClose) {
                return;
            }
            if (JoinSeats.Count() != InningeGame.NotEmptySeats().Count()) {
                NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "还有玩家未下注,不能发牌"));
                return;
            }
            IsChipinClose = true;
            PokerManager.Riffile();
            for (int i = 0; i < JoinSeats.Count(); i++) {
                ThreeCards threeCards = new ThreeCards(PokerManager.TackOut(3));
                JoinSeats[i].Pokers = threeCards;
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
        public void Look(int playerId) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (seat is null) {
                NotifySinglePlayer(WebscoketSendObjs.RoomMessage(playerId, "还未押底,不能查看!"), playerId);
                return;
            }
            if (seat.Pokers is null || seat.Pokers.Cards.Count != 3) {
                NotifySinglePlayer(WebscoketSendObjs.RoomMessage(playerId, "还未发牌,不能查看!"), playerId);
                return;
            }
            seat.LookOneCard();
            //int i;
            //Card card = null;
            //for (i = 0; i < 3; i++) {
            //    card = seat.PokersShow[i];
            //    if (card.Name == "") {
            //        card = GetSeatGetPokers(seat).Cards[i];
            //        seat.PokersShow[i] = card;
            //        break;
            //    }
            //}
            //seat.IsLooked = true;
            NotifySinglePlayer(new FreshGameFace(playerId), playerId);
        }
        /// <summary>
        /// 玩家天眼查看其他玩家一张牌
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="otherPlayerId"></param>
        /// <returns></returns>
        public object LookOthersPoker(int playerId, int otherPlayerId) {
            Seat mySeat = GetJionSeatByPlayerId(playerId);
            var Poker = new Poker();
            if (mySeat.PlayerIdWhichCanSee != 0) {
                return Poker;
            }
            else {
                Seat otherSeat = GetJionSeatByPlayerId(otherPlayerId);
                Card card = otherSeat.PokerOtherCanSee;
                Poker = new Poker(card.CardColor, card.Name, card.ComparedValue);
                return Poker;
            }
        }
        /// <summary>
        /// 玩家表态_暗注
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="amount"></param>
        public void ChipInNoLook(int playerId, decimal amount) {
            if (IsAmountLessThanPre(amount)) {
                return;
            }
            Seat seat = GetJionSeatByPlayerId(playerId);
            //if (seat.IsLooked || IsNotPassedCheck(seat)) {
            //    return;
            //}
            //if (IsDecutMoneySuccess(seat.IPlayer, amount)) {
            //    MoveToNextSeat(amount, false,playerId, EChipinType.NoLook);
            //}
            if (IsNotCurrentSeat(seat)) {
                return;
            }
            if (seat.ChipIn(amount,IsDecutMoneySuccess,EChipinType.NoLook)){
                MoveToNextSeat(amount, false, playerId, EChipinType.NoLook);
            }
        }
        /// <summary>
        /// 玩家表态_跟
        /// </summary>
        /// <param name="playerId"></param>
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
            if (seat.ChipIn(chipInAmount,IsDecutMoneySuccess,EChipinType.Follow)) {
                MoveToNextSeat(chipInAmount, iHadLook, playerId, EChipinType.Follow);
            }
        }
        /// <summary>
        ///  玩家放弃
        /// </summary>
        /// <param name="playerId"></param>
        public void Giveup(int playerId) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (IsNotPassedCheck(seat)) {
                return;
            }
            // seat.IsGaveUp = true;
            seat.Giveup();
            if (FirstSeat == seat) {
                FirstSeat = GetNextSeat(seat);
            }
            MoveToNextSeat(PreSeatAmount, PreSeatIsLook,playerId);
        }
        /// <summary>
        /// 玩家表态_直封
        /// </summary>
        /// <param name="playerId"></param>
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
            if (seat.ChipIn(chipInAmount,IsDecutMoneySuccess,EChipinType.Limit)) {
                MoveToNextSeat(chipInAmount, iHadLook, playerId, EChipinType.Limit);
            }
        }
        /// <summary>
        /// 玩家表态_加倍
        /// </summary>
        /// <param name="playerId"></param>
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
            if (seat.ChipIn(chipInAmount,IsDecutMoneySuccess,EChipinType.Double)) {
                MoveToNextSeat(chipInAmount, iHadLook, playerId, EChipinType.Double);
            }
        }
        /// <summary>
        /// 玩家表态_自定义积分
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="amount"></param>
        public object  ChipIn(int playerId, decimal amount) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (IsNotPassedCheck(seat)) {
                return new {
                    rl = ""
                };
            }
            bool iHadLook = seat.IsLooked;
            if (PreSeatIsLook&&!iHadLook) {
                if (IsAmountLessThanPre(amount*2)) {
                    return new {
                        rl = "金额有误,不能低于上家金额"
                    };
                }
            }
            if (!PreSeatIsLook&&iHadLook) {
                if (IsAmountLessThanPre(amount/2)) {
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
            if (seat.ChipIn(amount,IsDecutMoneySuccess,EChipinType.ChipIn)) {
                MoveToNextSeat(amount, iHadLook, playerId, EChipinType.ChipIn);
            }
            return new {
                rl = 1
            };
        }
        private bool IsAmountLessThanPre(decimal amount) {
            if (amount < PreSeatAmount) {
                return true;
            }
            return false;
        }
        /// <summary>
        ///开牌
        /// </summary>
        /// <param name="playerId"></param>
        public void Compare(int playerId) {
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
            WinnerSeat = GetWinner(seat, opponetSeat);
            WinnerSeat.IPlayer.DecutMoney(-CurrentTotal);
            IsWinnerStage = true;
            FirstSeat = WinnerSeat;//新庄家
            CurrentSeat = WinnerSeat;
            NotifyRoomPlayers(new FreshGameFace(0));
            InningeGame.GameOver(false, false);
        }
        //结算大牌积分
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
        /// 到最大轮次自动开牌
        /// </summary>
        private void CompareAll() {
            //  List<Seat> seats = JoinSeats.Where(s => s.IPlayer.Id != CurrentSeat.IPlayer.Id).ToList();
            // List<Seat> seats = JoinSeats;
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
            WinnerSeat.IPlayer.DecutMoney(-CurrentTotal);
            IsWinnerStage = true;
            FirstSeat = WinnerSeat;//新庄家
            CurrentSeat = WinnerSeat;
            NotifyRoomPlayers(new FreshGameFace(0));
            InningeGame.GameOver(false, false);//触发游戏结束事件
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
        private Seat GetNextSeat(Seat seat) {
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
        /// 轮到下一个玩家表态
        /// </summary>
        private void MoveToNextSeat(decimal amount, bool isLook,int playerId, EChipinType chipinType = EChipinType.Nothing) {
            lock (this) {
               PreSeatAmount = amount;
               CurrentSeat.PreChipInAmount = amount;
               CurrentSeat.PreChipType = chipinType;
                AddCurrentTotal();
                Seat tempSeat = CurrentSeat;
                do {
                    tempSeat = GetNextSeat(tempSeat);
                } while (tempSeat.IsGaveUp);
                CurrentSeat = tempSeat;
                PreSeatIsLook = isLook;
                if (CurrentSeat == FirstSeat) {
                    CurrentTurn++;
                }
                if (CurrentTurn > DefaultTurnCount) {
                    CompareAll();
                }
                if (JoinSeats.Where(s => s.IsGaveUp == false).Count() == 1) {
                    CompareAll();
                }
            }
            CheckDateTime = DateTime.Now;
            NotifyRoomPlayers(new ChipinAnimation(0,amount, playerId, chipinType));
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
            if (IsWinnerStage) {
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
        #endregion
    }

}
