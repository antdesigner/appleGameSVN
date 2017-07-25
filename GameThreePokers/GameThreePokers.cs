using AntDesigner.NetCore.GameCity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        /// 押底关闭
        /// </summary>
        private bool IsChipinClose { get; set; }
        #endregion
        public GameThreePokers() {
            #region 初始化时每项游戏必须设置的属性
            ShowName = "炸金花";//游戏名称,可根据游戏项目指定属性值
            Name = this.GetType().Name;//反射调用名称(勿修改)
            PlayerCountLimit = 5;//人数上限,可根据游戏项目指定属性值
            PlayerCountLeast = 2;//人数下限,可根据游戏项目指定属性值
            #endregion
            #region 自定义初始化区域
            ChipInAmount = 1;
            DefaultTurnCount = 10;
            LimitAmount = 50;//封顶
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
            NotifyRoomPlayers(WebscoketSendObjs.RoomMessage(0, "游戏开始了!请开始押底!"));
        }
        /// <summary>
        /// 初始化每局公共信息
        /// </summary>
        private void InitPublicInfo() {
            CurrentTotal = 0;//初始化
            CurrentTurn =1;//初始化轮次
            JoinSeats = new List<Seat>();//次序控制
            PreSeatIsLook = false;//上家是否看牌
            PreSeatAmount = ChipInAmount;//最近下注积分
            IsChipinClose = false;
            WinnerSeat = null;
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
            List<object> seats = new List<object>();
            int n = 0;
            Seat mySeat= GetJionSeatByPlayerId(playerId);
            for (int i = 0; i < JoinSeats.Count; i++) {
                var seat = JoinSeats[i];
                var PokersShow = new List<Card>();
                for (int k = 0; k < 3; k++) {
                    PokersShow.Add(new Card(0, "", ""));
                }
                int j = n++;
                if (seat.IPlayer.Id == playerId) {
                    j = 4;
                    PokersShow = seat.PokersShow;
                }
                if (!(WinnerSeat is null)&&(!seat.IsGaveUp)) {
                    PokersShow = seat.Pokers.CardsSort;
                }
                var PokerOtherCanSee = new Card(0, "", "");
                if (!(mySeat is null)&&seat.IPlayer.Id==mySeat.PlayerIdWhichCanSee) {
                    PokerOtherCanSee = seat.PokerOtherCanSee;
                    seat.PokersShow[2] = PokerOtherCanSee;
                }
                var seatInfo = new {
                    canvId = j,
                    playerId = playerId,
                    /// 可以看的牌
                    PokersShow = PokersShow,
                    ///别人可以看的牌Q
                    PokerOtherCanSee =PokerOtherCanSee,
                    /// 可以查看其他玩家的一张牌的Id
                    PlayerIdWhichCanSee = seat.PlayerIdWhichCanSee,
                    /// 自己上一次下注积分
                    PreChipInAmount = seat.PreChipInAmount,
                    /// 是否已押底
                    IsChipIned = seat.IsChipIned,
                    /// 是否已看牌
                    IsLooked = seat.IsLooked,
                    /// 是否已弃牌
                    IsGaveUp = seat.IsGaveUp
                };
                seats.Add(seatInfo);
            }
            int winnerid = 0;
            if (null != this.WinnerSeat) {
                winnerid = WinnerSeat.IPlayer.Id;
            }
            var PublicInfo = new {
                /// 每次打底(入局)积分
                ChipInAmount = this.ChipInAmount,
                /// 封顶积分
                LimitAmount = this.LimitAmount,
                /// 每局游戏轮次
                DefaultTurnCount = this.DefaultTurnCount,
                /// 一局的起始位置
                FirstSeatPlayerId = this.FirstSeat.IPlayer.Id,
                /// 但前底池积分
                CurrentTotal = this.CurrentTotal,
                /// 当前轮次
                CurrentTurn = this.CurrentTurn,
                /// 上家是否看牌
                PreSeatIsLook = this.PreSeatAmount,
                /// 上家下注积分
                PreSeatAmount = this.PreSeatAmount,
                /// 当前座位
                CurrentSeatPlayId = this.CurrentSeat.IPlayer.Id,
                /// 赢家座位
                WinnerSeatPlayerId = winnerid,
                ///打底关闭
                IsChipinClose = this.IsChipinClose
            };
            var GameFace = new {
                PublicInfo = PublicInfo,
                Seats = seats
            };
            return GameFace;
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
                return;
            }
            Seat seat = GetRoomSeatByPlayerId(playerId);
            if (IsDecutMoneySuccess(seat.IPlayer, ChipInAmount)) {
                AddPlaySeat(seat);
                seat.IsChipIned = true;
                CurrentTotal = CurrentTotal + ChipInAmount;//增加锅底金额
            }
            NotifyRoomPlayers(new FreshGameFace(0));
        }
        /// <summary>
        /// 发牌
        /// </summary>
        public void Deal() {
            if (IsChipinClose) {
                return;
            }
            IsChipinClose = true;
            PokerManager.Riffile();
            for (int i = 0; i < JoinSeats.Count(); i++) {
                ThreeCards threeCards = new ThreeCards(PokerManager.TackOut(3));
                JoinSeats[i].Pokers = threeCards;
            }
            NotifyRoomPlayers(new FreshGameFace(0));
        }
        /// <summary>
        /// 玩家看牌一张
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public void Look(int playerId) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            int i;
            Card card = null;
            for (i = 0; i < 3; i++) {
                card = seat.PokersShow[i];
                if (card.Name == "") {
                    card = GetSeatGetPokers(seat).Cards[i];
                    seat.PokersShow[i] = card;
                    break;
                }
            }
            seat.IsLooked = true;
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
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (seat.IsLooked || IsNotPassedCheck(seat)) {
                return;
            }
            if (IsDecutMoneySuccess(seat.IPlayer, amount)) {
                MoveToNextSeat(amount, false);
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
            if (IsDecutMoneySuccess(seat.IPlayer, chipInAmount)) {
                MoveToNextSeat(chipInAmount, iHadLook);
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
            seat.IsGaveUp = true;
            MoveToNextSeat(PreSeatAmount, PreSeatIsLook);
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
            if (IsDecutMoneySuccess(seat.IPlayer, chipInAmount)) {
                MoveToNextSeat(chipInAmount, iHadLook);
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
            if (IsDecutMoneySuccess(seat.IPlayer, chipInAmount)) {
                MoveToNextSeat(chipInAmount, iHadLook);
            }

        }
        /// <summary>
        /// 玩家表态_自定义积分
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="amount"></param>
        public void ChipIn(int playerId, decimal amount) {
            Seat seat = GetJionSeatByPlayerId(playerId);
            if (IsNotPassedCheck(seat)) {
                return;
            }
            bool iHadLook = seat.IsLooked;
            if (amount > LimitAmount) {
                amount = LimitAmount;
            }
            if (IsDecutMoneySuccess(seat.IPlayer, amount)) {
                MoveToNextSeat(amount, iHadLook);
            }

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
            WinnerSeat.IPlayer.DecutMoney(CurrentTotal);
            FirstSeat = WinnerSeat;//新庄家
            NotifyRoomPlayers(new FreshGameFace(0));
        }
        //结算大牌积分
        public void SettolAccount() {
        }
        /// <summary>
        /// 玩家押底入局
        /// </summary>
        /// <param name="seat"></param>
        private void AddPlaySeat(Seat seat) {
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
                player.DecutMoney(-amount);
                return true;
            }
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
            if  (!(seat.Pokers is null)) {
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
            List<Seat> seats = JoinSeats.Where(s => s.IPlayer.Id != CurrentSeat.IPlayer.Id).ToList();
            List<Seat> notGaveupSeats = new List<Seat>();
            for (int i = 0; i < seats.Count; i++) {
                if (seats[i].IsGaveUp) {
                    continue;
                }
                notGaveupSeats.Add(seats[i]);
            }
            WinnerSeat = (notGaveupSeats.OrderBy(s => s.Pokers).ToList())[0];
            WinnerSeat.IPlayer.DecutMoney(CurrentTotal);
            FirstSeat = WinnerSeat;//新庄家
            NotifyRoomPlayers(new FreshGameFace(0));
           // InningeGame.GameOver();//触发游戏结束事件
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
        private void MoveToNextSeat(decimal amount, bool IsLook) {
            PreSeatAmount = amount;
            CurrentSeat.PreChipInAmount = amount;
            CurrentTotal = CurrentTotal + amount;
            Seat tempSeat = CurrentSeat;
            do {
                tempSeat = GetNextSeat(tempSeat);
            } while (tempSeat.IsGaveUp);
            CurrentSeat = tempSeat;
            PreSeatIsLook = IsLook;
            if (CurrentSeat == FirstSeat) {
                CurrentTurn++;
            }
            if (CurrentTurn>DefaultTurnCount) {
                CompareAll();
            }
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
            if (IsNotCurrentSeat(seat)) {
                return true;
            }
            if (!seat.IsChipIned || seat.IsGaveUp) {
                return true;
            }
            if (!(WinnerSeat is null)) {
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
