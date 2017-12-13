using AntDesigner.NetCore.GameCity;
using System;
using System.Collections.Generic;
namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    public class Seat : GameCity.Seat {
        /// <summary>
        /// 下家
        /// </summary>
        public Seat NextSeat { get; set; }
        /// <summary>
        /// 上家
        /// </summary>
        public Seat PreSeat { get; set; }
        /// <summary>
        /// 是否飘起
        /// </summary>
        bool IsPiao { get; set; }
        /// <summary>
        /// 操作令牌
        /// </summary>
        internal OptionToken Token { get; private set; }
        /// <summary>
        /// 座位状态
        /// </summary>
        internal SeatStage Stage { get; set; }
        internal event EventHandler PayedCard;
        internal event EventHandler GetedCard;
        internal Action<Seat> GetCardAction;
        internal event EventHandler DidOption;
        internal event EventHandler EjectionedToken;
        public Seat(IInningeGame inningeGame) : base(inningeGame) {
            Stage = SeatStage.Starting;
            IsPiao = false;

        }
        /// <summary>
        /// 玩家手上牌
        /// </summary>
        internal HandCardManager HandCards { get; set; }
        internal void InitialHandCars(StandCardsCollection maJiangs) {
            HandCards.StandCards = maJiangs;
            Stage = SeatStage.Waiting;
        }
        /// <summary>
        /// 摸牌
        /// </summary>
        public void PickedCard(Func<Card> pickedCard) {
            if (Token is null) {
                throw new Exception("没有获得操作令牌,操作非法!");
            }
            HandCards.SetCardPicked(pickedCard());
            OnGetedCard();
        }
        /// <summary>
        /// 出牌
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cardColor"></param>
        /// <returns></returns>
        internal Card PayCard(string name,string cardColor) {
           Card card= HandCards.FromStandCardsFind(name, cardColor);
           HandCards.PutOuCard(card);
           OnPayedCard(card);
           return card;
        }
        /// <summary>
        /// 暂停
        /// </summary>
        void Pause() {
            Stage = SeatStage.Paused;
        }
        /// <summary>
        /// 暂停取消
        /// </summary>
        void CancelPause() {
            Stage = SeatStage.Waiting;
        }
        /// <summary>
        /// 出牌后事件
        /// </summary>
        /// <param name="card"></param>
        void OnPayedCard(Card card) {
            EjectionTakon(card);
            PayedCard?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// 得牌后事件
        /// </summary>
        void OnGetedCard() {
            Stage = SeatStage.Paying;
            GetedCard?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// 操作执行后事件
        /// </summary>
        /// <param name="optionor"></param>
        void OnDidOption(Optionor optionor) {
            DidOption?.Invoke(this, new OptionorEventArgs(optionor));
        }
        /// <summary>
        /// 抛出令牌事件
        /// </summary>
        void OnEjectionedToken() {
            EjectionedToken?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="optionor"></param>
        public void Do(Optionor optionor) {
            if (ExistOptionor(optionor)) {
                bool needOtherCard = optionor.Do(HandCards);
                if (needOtherCard) {
                    GetCardAction?.Invoke(this);
                }
                OnDidOption(optionor);
            }
        }
        /// <summary>
        /// 是否可以执行该操作
        /// </summary>
        /// <param name="optionor"></param>
        /// <returns></returns>
        private bool ExistOptionor(Optionor optionor) {
            return HandCards.Optionors.HaseOptionor(optionor);
        }
       internal void SetToken(OptionToken token){
            Token = token;
        }
        /// <summary>
        /// 抛出令牌
        /// </summary>
        /// <param name="card"></param>
        internal void EjectionTakon(Card card) {
            Token.EjectionSeat = this;
            Token.SnatchCard = card;
            OnEjectionedToken();
        }
    }
}