using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 麻将牌容器
    /// </summary>
    public class MaJiangCollection : List<Card> {
        public Card FindCard(string name, string cardColor) {
            Card card = null;
            card = this.Find(c => c.Name == name && c.CardColor == cardColor);
            return card;
        }
        /// <summary>
        /// 获得一张牌
        /// </summary>
        /// <param name="cardM"></param>
        /// <returns></returns>
        public Card FindCard(CardModel cardM) {
            Card card = null;
            card = this.Find(c => c.Name ==cardM.Name && c.CardColor == cardM.CardColor);
            return card;
        }
        /// <summary>
        /// 找出相同数量的牌
        /// </summary>
        /// <param name="n">相同数量</param>
        /// <returns>有相同数量牌的集合</returns>
        public MaJiangCollection FindTheSameCards(int n) {
            MaJiangCollection cards = new MaJiangCollection();
            for (int i = 0; i < this.Count; i++) {
                var card = this[i];
                var theSameCards = this.FindAll(c => c.Name == card.Name && c.CardColor == card.CardColor);
                if (theSameCards.Count >= n && !cards.Exists(c => c.Name == card.Name && c.CardColor == card.CardColor)) {
                    cards.Add(card);
                }
            }
            return cards;
        }
        /// <summary>
        /// 是否顺牌,3个一样或顺子
        /// </summary>
        /// <param name="maJiangs"></param>
        /// <returns></returns>
        public bool IsThree() {
            if (this.Count != 3) {
                throw new ArgumentOutOfRangeException("不是3张牌");
            }
            if (this.FindAll(c => c.CardColor == this[0].CardColor).Count != 3) {
                return false;
            }
            if (this[0].Name == this[1].Name && this[0].Name == this[2].Name) {
                return true;
            }

            var maJiangCollection = this.SortByColorThenName();
            if (maJiangCollection[0].ComparedValue + 1 == maJiangCollection[1].ComparedValue
                && maJiangCollection[1].ComparedValue + 1 == maJiangCollection[2].ComparedValue) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 逆序是否顺牌,3个一样或顺子
        /// </summary>
        /// <param name="maJiangs"></param>
        /// <returns></returns>
        public bool IsThreeDesc() {
            if (this.Count != 3) {
                throw new ArgumentOutOfRangeException("不是3张牌");
            }
            if (this.FindAll(c => c.CardColor == this[0].CardColor).Count != 3) {
                return false;
            }
            if (this[0].Name == this[1].Name && this[0].Name == this[2].Name) {
                return true;
            }

            var tempMaJiangCollection = this.SortByColorThenNameDesc();
            if (tempMaJiangCollection[0].ComparedValue - 1 == tempMaJiangCollection[1].ComparedValue
                && tempMaJiangCollection[1].ComparedValue - 1 == tempMaJiangCollection[2].ComparedValue) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否3张一样
        /// </summary>
        /// <returns></returns>
        public bool IsThreeSame() {
            if (this.Count != 3) {
                throw new ArgumentOutOfRangeException("不是3张牌");
            }
            if (this.FindAll(c => c.CardColor == this[0].CardColor).Count != 3) {
                return false;
            }
            if (this[0].Name == this[1].Name && this[0].Name == this[2].Name) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否4张一样
        /// </summary>
        /// <returns></returns>
        public bool IsFourSame() {
            if (this.Count != 4) {
                throw new ArgumentOutOfRangeException("不是4张牌");
            }
            if (this.FindAll(c => c.CardColor == this[0].CardColor).Count != 4) {
                return false;
            }
            if (this[0].Name == this[1].Name
                && this[0].Name == this[2].Name
                && this[0].Name == this[3].Name) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否可以构成搭子,顺子或豹子
        /// </summary>
        /// <returns></returns>
        public bool IsMachtedCards() {
            if (this.Count != 2) {
                return false;
            }
            if (!this[0].IsTheSameColorOf(this[1])) {
                return false;
            }
            if (this[0].IsTheSameWith(this[1])) {
                return true;
            }
            var tempMaJiangCollection = this.SortByColorThenName();
            if (tempMaJiangCollection[0].ComparedValue + 1 == tempMaJiangCollection[1].ComparedValue
                || tempMaJiangCollection[0].ComparedValue + 2 == tempMaJiangCollection[1].ComparedValue) {

                return true;
            }
            return false;
        }
        /// <summary>
        /// 逆序是否可以构成搭子,顺子或豹子
        /// </summary>
        /// <returns></returns>
        public bool IsMachtedCardsDesc() {
            if (this.Count != 2) {
                return false;
            }
            if (!this[0].IsTheSameColorOf(this[1])) {
                return false;
            }
            if (this[0].IsTheSameWith(this[1])) {
                return true;
            }
            var tempMaJiangCollection = this.SortByColorThenNameDesc();
            if (tempMaJiangCollection[0].ComparedValue - 1 == tempMaJiangCollection[1].ComparedValue
                || tempMaJiangCollection[0].ComparedValue - 2 == tempMaJiangCollection[1].ComparedValue) {

                return true;
            }
            return false;
        }
        /// <summary>
        /// 获得搭子需要的牌
        /// </summary>
        /// <param name="maJiangs"></param>
        /// <returns></returns>
        public MaJiangCollection GetMatchedCards() {
            if (this.Count > 2) {
                throw new ArgumentOutOfRangeException("只能处理一到两张牌");
            }
            MaJiangCollection matchedMajiangs = new MaJiangCollection();
            if (this.Count == 1) {
                matchedMajiangs.Add(this[0]);
                return matchedMajiangs;
            }
            var tempMaJiangCollection = this.SortByColorThenName();
            var preCard = tempMaJiangCollection[0];
            var afterCard = tempMaJiangCollection[1];
            if (preCard.CardColor != afterCard.CardColor) {
                return matchedMajiangs;
            }
            if (preCard.Name == afterCard.Name) {
                matchedMajiangs.Add(preCard);
                return matchedMajiangs;
            }
            if (preCard.ComparedValue + 2 == afterCard.ComparedValue) {
                var card = CreatAfterCard(preCard);
                matchedMajiangs.Add(card);
                return matchedMajiangs;
            }
            if (preCard.ComparedValue + 1 == afterCard.ComparedValue) {
                if (preCard.ComparedValue == 0) {
                    Card card = CreatAfterCard(afterCard);
                    matchedMajiangs.Add(card);
                    return matchedMajiangs;

                }
                else if (afterCard.ComparedValue == 8) {
                    Card card = CreatPreCard(preCard);
                    matchedMajiangs.Add(card);
                    return matchedMajiangs;
                }
                else {
                    Card myPreCard = CreatPreCard(preCard);
                    Card myAfterCard = CreatAfterCard(afterCard);
                    matchedMajiangs.Add(myPreCard);
                    matchedMajiangs.Add(myAfterCard);
                    return matchedMajiangs;
                }
            }
            return matchedMajiangs;
        }
        /// <summary>
        /// 生成前一张牌
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Card CreatPreCard(Card card) {
            if (card.ComparedValue == 0) {
                throw new ArgumentOutOfRangeException("不能指定最小牌前面的牌");
            }
            Card preCard = new Card(card.ComparedValue - 1, card.CardColor, (card.ComparedValue).ToString());
            return preCard;
        }
        /// <summary>
        /// 生成后一张牌
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public Card CreatAfterCard(Card card) {
            if (card.ComparedValue == 8) {
                throw new ArgumentOutOfRangeException("不能指定最大牌后面的牌");
            }
            Card aftercard = new Card(card.ComparedValue + 1, card.CardColor, (card.ComparedValue + 2).ToString());
            return aftercard;
        }
        /// <summary>
        /// 去将,从牌中移除一对牌
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public MaJiangCollection RemoveCountOf(Card card, int count) {
            MaJiangCollection majiangs = new MaJiangCollection();
            int k = count;
            for (int j = 0; j < this.Count; j++) {
                if (this[j].IsTheSameWith(card) && k > 0) {
                    k--;
                    continue;
                }
                majiangs.Add(this[j]);
            }
            return majiangs;
        }
        /// <summary>
        /// 获得牌中不能组合成顺子或豹子的牌
        /// </summary>
        /// <returns></returns>
        public List<MaJiangCollection> GetMatchedCardsCollection() {
            var tempMaJiangCollection = this.SortByColorThenName();
            List<MaJiangCollection> matchedCardsColleciton = new List<MaJiangCollection>();
            MaJiangCollection checkCards = new MaJiangCollection();
            if (tempMaJiangCollection.Count == 1) {
                checkCards.Add(tempMaJiangCollection[0]);
                matchedCardsColleciton.Add(checkCards);
                return matchedCardsColleciton;
            }
            if (this.Count == 2) {
                checkCards.Add(tempMaJiangCollection[0]);
                checkCards.Add(tempMaJiangCollection[1]);
                if (checkCards.IsMachtedCards()) {
                    matchedCardsColleciton.Add(checkCards);
                }
                else {
                    checkCards.RemoveAt(1);
                }
                return matchedCardsColleciton;
            }
            int k = 2;
            for (int i = 0; i < tempMaJiangCollection.Count; i++) {
                MaJiangCollection myCheckCards = new MaJiangCollection();
                if (k == 0) {
                    break;
                }
                if (this.Count - i == 1) {
                    myCheckCards.Add(tempMaJiangCollection[i]);
                    matchedCardsColleciton.Add(myCheckCards);
                    return matchedCardsColleciton;
                }
                if (this.Count - i == 2) {
                    myCheckCards.Add(tempMaJiangCollection[i]);
                    myCheckCards.Add(tempMaJiangCollection[i + 1]);
                    if (!myCheckCards.IsMachtedCards()) {
                        myCheckCards.RemoveAt(1);
                        matchedCardsColleciton.Add(myCheckCards);
                        k--;
                        continue;
                    }
                    else {
                        matchedCardsColleciton.Add(myCheckCards);
                        k--;
                        return matchedCardsColleciton;
                    }
                }
                myCheckCards.Add(tempMaJiangCollection[i]);
                myCheckCards.Add(tempMaJiangCollection[i + 1]);
                myCheckCards.Add(tempMaJiangCollection[i + 2]);
                if (myCheckCards.IsThree()) {
                    i = i + 2;
                }
                else {
                    myCheckCards.RemoveAt(2);
                    i = i + 1;
                    if (!myCheckCards.IsMachtedCards()) {
                        myCheckCards.RemoveAt(1);
                        i = i - 1;
                    }
                    matchedCardsColleciton.Add(myCheckCards);
                    k--;
                }
            }
            return matchedCardsColleciton;
        }
        /// <summary>
        /// 逆序获得牌中不能组合成顺子或豹子的牌
        /// </summary>
        /// <returns></returns>
        public List<MaJiangCollection> GetMatchedCardsDescCollection() {
            var tempMaJiangCollection = this.SortByColorThenNameDesc();
            List<MaJiangCollection> matchedCardsColleciton = new List<MaJiangCollection>();
            MaJiangCollection checkCards = new MaJiangCollection();
            if (tempMaJiangCollection.Count == 1) {
                checkCards.Add(tempMaJiangCollection[0]);
                matchedCardsColleciton.Add(checkCards);
                return matchedCardsColleciton;
            }
            if (this.Count == 2) {
                checkCards.Add(tempMaJiangCollection[0]);
                checkCards.Add(tempMaJiangCollection[1]);
                if (checkCards.IsMachtedCards()) {
                    matchedCardsColleciton.Add(checkCards);
                }
                else {
                    checkCards.RemoveAt(1);
                }
                return matchedCardsColleciton;
            }
            int k = 2;
            for (int i = 0; i < tempMaJiangCollection.Count; i++) {
                MaJiangCollection myCheckCards = new MaJiangCollection();
                if (k == 0) {
                    break;
                }
                if (tempMaJiangCollection.Count - i == 1) {
                    myCheckCards.Add(tempMaJiangCollection[i]);
                    matchedCardsColleciton.Add(myCheckCards);
                    return matchedCardsColleciton;
                }
                if (tempMaJiangCollection.Count - i == 2) {
                    myCheckCards.Add(tempMaJiangCollection[i]);
                    myCheckCards.Add(tempMaJiangCollection[i + 1]);
                    if (!myCheckCards.IsMachtedCardsDesc()) {
                        myCheckCards.RemoveAt(1);
                        matchedCardsColleciton.Add(myCheckCards);
                        k--;
                        continue;
                    }
                    else {
                        matchedCardsColleciton.Add(myCheckCards);
                        k--;
                        return matchedCardsColleciton;
                    }

                }
                myCheckCards.Add(tempMaJiangCollection[i]);
                myCheckCards.Add(tempMaJiangCollection[i + 1]);
                myCheckCards.Add(tempMaJiangCollection[i + 2]);
                if (myCheckCards.IsThreeDesc()) {
                    i = i + 2;
                }
                else {
                    myCheckCards.RemoveAt(2);
                    i = i + 1;
                    if (!myCheckCards.IsMachtedCardsDesc()) {
                        myCheckCards.RemoveAt(1);
                        i = i - 1;
                    }
                    matchedCardsColleciton.Add(myCheckCards);
                    k--;
                }
            }
            return matchedCardsColleciton;
        }
        /// <summary>
        /// 排序
        /// </summary>
        public MaJiangCollection SortByColorThenName() {
            MaJiangCollection majiangs = new MaJiangCollection();
            var temp = this.OrderBy(c => c.CardColor).ThenBy(c => c.Name).ToList<Card>();
            majiangs.AddRange(temp);
            return majiangs;
        }
        /// <summary>
        /// 排序
        /// </summary>
        public MaJiangCollection SortByColorThenNameDesc() {
            MaJiangCollection majiangs = new MaJiangCollection();
            var temp = this.OrderByDescending(c => c.CardColor).ThenByDescending(c => c.Name).ToList<Card>();
            majiangs.AddRange(temp);
            return majiangs;
        }
        /// <summary>
        /// 获得相同的牌n张
        /// </summary>
        /// <param name="card"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public MaJiangCollection GetSameCards(Card card, int n) {
            MaJiangCollection majians = new MaJiangCollection();
            var cards = this.FindAll(c => c.IsTheSameWith(card));
            if (cards.Count >= n) {
                for (int i = 0; i < 3; i++) {
                    majians.Add(cards[i]);
                }
                return majians;
            }
            return null;
        }
        /// <summary>
        /// 获得相同的牌n张
        /// </summary>
        /// <param name="card"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public MaJiangCollection GetSameCards(CardModel card, int n) {
            MaJiangCollection majians = new MaJiangCollection();
            var cards = this.FindAll(c => c.IsTheSameWith(card));
            if (cards.Count >= n) {
                for (int i = 0; i < 3; i++) {
                    majians.Add(cards[i]);
                }
                return majians;
            }
            return null;
        }

    }
}

