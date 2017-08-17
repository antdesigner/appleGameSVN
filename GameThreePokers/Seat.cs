using AntDesigner.NetCore.GameCity;
using System;
using System.Collections.Generic;
namespace AntDesigner.NetCore.Games.GameThreePokers {
    /// <summary>
    /// 该游戏项目的特殊化对象,添加属性来保存该游戏项目的座位属性和特殊业务逻辑
    /// </summary>
    public class Seat : GameCity.Seat {
        public Seat(IInningeGame inningeGame) : base(inningeGame) {
            PokersShow = new List<Card>();
            InitialPokerShow();
            PokerOtherCanSee = null;
            PreChipInAmount = 0;
            IsChipIned = false;
            IsLooked = false;
            IsGaveUp = false;
        }
        /// <summary>
        /// 得到的三张牌
        /// </summary>
        public ThreeCards Pokers { get; set; }
        /// <summary>
        /// 可以看的牌
        /// </summary>
        public List<Card> PokersShow { get; set; }
        public Card PokerOtherCanSee { get; set; }
        /// <summary>
        /// 可以查看其他玩家的一张牌的Id
        /// </summary>
        public int PlayerIdWhichCanSee { get; set; }
        /// <summary>
        /// 自己上一次下注积分
        /// </summary>
        public decimal PreChipInAmount { get; set; }
        /// <summary>
        /// 打底金额
        /// </summary>
        public decimal ChipinAmount { get; set; }
        /// <summary>
        /// 下注方式
        /// </summary>
        public EChipinType PreChipType { get; set; }
        /// <summary>
        /// 是否已押底
        /// </summary>
        public bool IsChipIned { get; set; }
        /// <summary>
        /// 是否已看牌
        /// </summary>
        public bool IsLooked { get; set; }
        /// <summary>
        /// 是否已弃牌
        /// </summary>
        public bool IsGaveUp { get; set; }
        /// <summary>
        /// 清空座位每局数据
        /// </summary>
        public override void ClearSeatInfo() {
            Pokers = null;
            PokersShow.Clear();
            InitialPokerShow();
            PreChipInAmount = 0;
            PreChipType = EChipinType.Nothing;
            IsChipIned = false;
            IsLooked = false;
            IsGaveUp = false;
            PlayerIdWhichCanSee = 0;
            PokerOtherCanSee = null;
        }
        private void InitialPokerShow() {
            for (int i = 0; i < 3; i++) {
                PokersShow.Add(new Card(0, "", ""));
            }
        }
        public bool PlayerChipin(decimal amount, Func<IPlayerJoinRoom, decimal, bool> DDecutMoney) {
            if (IsChipIned) {
                return false;
            }
            if (DDecutMoney(this.IPlayer, amount)) {
                IsChipIned = true;
                ChipinAmount = amount;
                PreChipInAmount = amount;
                return true;
            }
            return false;
        }
        public void LookOneCard() {
            int i;
            Card card = null;
            for (i = 0; i < 3; i++) {
                card = PokersShow[i];
                if (card.Name == "") {
                    card = GetSeatGetPokers(this).Cards[i];
                    PokersShow[i] = card;
                    break;
                }
            }
            IsLooked = true;
        }
        private ThreeCards GetSeatGetPokers(Seat seat) {
            if (!(seat.Pokers is null)) {
                return seat.Pokers;
            }
            return null;
        }
        public void Giveup() {
            IsGaveUp = true;
        }
        public Card GetOtherCanSeePoker() {
            if (null != this.PokerOtherCanSee) {
                return this.PokerOtherCanSee;
            }
           this.PokerOtherCanSee = this.Pokers.Cards[0];
            return this.PokerOtherCanSee;
        }
        public bool ChipIn(decimal amount, Func<IPlayerJoinRoom, decimal, bool> DDecutMoney, EChipinType chipType) {
            if (chipType==EChipinType.NoLook&&this.IsLooked) {
                return false;
            }
            if (DDecutMoney(this.IPlayer, amount)) {
                PreChipInAmount = amount;
                PreChipType = chipType;
                return true;
            }
            return false;
        }
    }
}
