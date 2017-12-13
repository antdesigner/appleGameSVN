using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 手上的牌
    /// </summary>
    public class HandCardManager {
        /// <summary>
        /// 站着的牌
        /// </summary>
        internal StandCardsCollection StandCards { get; set; }
        /// <summary>
        /// 躺下的牌(碰,杠之后亮在一边的牌)
        /// </summary>
        internal SleepMaJiangsCollection SleepCards { get; set; }
        /// <summary>
        /// 已出的牌
        /// </summary>
        PutOutCardsCollection  PutOutCards { get; set; }
        /// <summary>
        /// 待操作的牌
        /// </summary>
        public Card CardPicked { get; private set; }
        /// <summary>
        /// 可以要的牌操作集合
        /// </summary>
        public OptionorCollection Optionors { get; set; }
        Func<Card> GetCardAction { get; set; }
        HandCardManager() {
            StandCards = new StandCardsCollection();
            SleepCards = new SleepMaJiangsCollection();
            Optionors = new OptionorCollection();
        }
        /// <summary>
        /// 更新可要的牌
        /// </summary>
        internal void FreshOptionors(Func<HandCardManager, OptionorCollection> freshOptionorsAction) {

            Optionors = freshOptionorsAction.Invoke(this);
        }
        /// <summary>
        /// 取得此牌的可操作集合
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        OptionorCollection  GetOptionersWith(Card card) {
          var myOptionors= Optionors.GetOptionorsOf(card);
            return myOptionors;
        }
        /// <summary>
        /// 移除牌
        /// </summary>
        /// <param name="card"></param>
        internal void Romove(Card card) {
            StandCards.Remove(card);
        }
        /// <summary>
        /// 摸牌
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cardColor"></param>
        /// <returns></returns>
        internal Card GetCard(string name, string cardColor) {
            Card card = GetCardAction?.Invoke();
            SetCardPicked(card);
            return card;
        }
        internal void SetCardPicked(Card card) {
            CardPicked = card;
        }

        /// <summary>
        /// 能碰的牌
        /// </summary>
        /// <returns></returns>
        public MaJiangCollection GetPengCards() {
            return StandCards.FindTheSameCards(2);
        }
        /// <summary>
        /// 能杠的牌
        /// </summary>
        /// <returns></returns>
        public MaJiangCollection GetGangCards() {
            return StandCards.FindTheSameCards(3);
        }
        /// <summary>
        /// 清空要牌列表
        /// </summary>
        public void ClearOptionors() {
            Optionors.Clear();
        }
        /// <summary>
        /// 添加要牌
        /// </summary>
        /// <param name="option"></param>
        public void AddOptionor(Optionor option) {
            Optionors.Add(option);
        }
        /// <summary>
        /// 和代操作的牌相同
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool IstheSameWhithPikedCard(Card card) {
            if (CardPicked is null) {
                return false;
            }
            if (CardPicked.Name == card.Name && CardPicked.CardColor == card.CardColor) {
                return true;
            }
            return false;
        }
        public bool IstheSameWhithPikedCard(CardModel card) {
            if (CardPicked is null) {
                return false;
            }
            if (CardPicked.Name == card.Name && CardPicked.CardColor == card.CardColor) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 出牌
        /// </summary>
        /// <param name="card"></param>
        internal void PutOuCard(Card card) {
            if (!(CardPicked is null)) {
                StandCards.Add(CardPicked);
                CardPicked = null;
            }
            var myCard = StandCards.Find(c => c.IsTheSameWith(card));
            if (!(myCard is null)) {
                StandCards.Remove(myCard);
                PutOutCards.Add(myCard);
            }
        }
        /// <summary>
        /// 从手中的牌挑出指定牌
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cardColor"></param>
        /// <returns></returns>
        internal Card FromStandCardsFind(string name,string cardColor) {
            Card card = null;
            card=StandCards.FindCard(name, cardColor);
            return card;
        }
    }
}
