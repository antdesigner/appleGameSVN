using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameThreePokers {
   public class ThreeCards:IComparable, IComparable<ThreeCards>, IEquatable<ThreeCards> {
        public List<Card> Cards { get; set; }
        public List<Card> CardsSort { get; private set; }
        public ECardsType CardsType { get;private set; }
        public ThreeCards(List<Card> cards) {
            Cards = new List<Card>();
           CardsSort = new List<Card>();
            if (cards.Count!=3) {
                throw new Exception("ThreeCards的参数不是包含有3个元素");
            }
            for (int i = 0; i < 3; i++) {
                Cards.Add(cards[i]);
            }
            for (int i = 0; i < 3; i++) {
                CardsSort.Add(cards[i]);
            }
            CardsSort.Sort();
            CardsType = GetCardsType();
        }
        public int CompareTo(object obj) {
            if (!(obj is ThreeCards)) {
                throw new InvalidOperationException("CompareTo: Not a ThreeCards");
            }
            ThreeCards compareCards = (ThreeCards)obj;
            if (this.CardsType==compareCards.CardsType) {
                return CompareName(compareCards);
            }
            else {
                if (this.CardsType - compareCards.CardsType > 0) {
                    return 1;
                }
                else if (this.CardsType - compareCards.CardsType < 0) { 
                    return -1;
                }
                return 0;
            }

        }
        public int CompareTo(ThreeCards cards) {
            return CompareTo((object)cards);
        }
        public static bool operator <(ThreeCards cards1, ThreeCards cards2) {
            return cards1.CompareTo(cards2) < 0;
        }
        public static bool operator >(ThreeCards cards1, ThreeCards cards2) {
            return cards1.CompareTo(cards2) > 0;
        }
        public override int GetHashCode() {
            return CardsSort.GetHashCode();
        }
        public bool Equals(ThreeCards cards) {
            if (this.CardsType==cards.CardsType&&this.CompareName(cards)==0 ){
                return true;
            }
            return false;
        }
        public override bool Equals(object cards) {

            if (!(cards is ThreeCards))
                throw new InvalidOperationException("CompareTo: Not a ThreeCards");
            return Equals((ThreeCards)cards);
        }
        public static bool operator ==(ThreeCards n1, ThreeCards n2) {
            return n1.Equals(n2);
        }
        public static bool operator !=(ThreeCards n1, ThreeCards n2) {
            return !(n1 == n2);
        }
        /// <summary>
        /// 只比较点数大小
        /// </summary>
        /// <param name="compareCards"></param>
        /// <returns></returns>
        private int CompareName(ThreeCards compareCards) {
            if (this.CardsType == ECardsType.Tractor && compareCards.CardsType == ECardsType.Tractor) {
                for (int i = 1; i > 0; i--) {
                    if (CardsSort[i] > compareCards.CardsSort[i]) {
                        return 1;
                    }
                    else if (CardsSort[i] < compareCards.CardsSort[i]) {
                        return -1;
                    }
                }
                return 0;
            }
            for (int i = 2; i > 0; i--) {
                if (this.CardsSort[i] > compareCards.CardsSort[i]) {
                    return 1;
                }
                else if (this.CardsSort[i] < compareCards.CardsSort[i]) {
                    return -1;
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取牌的类型
        /// </summary>
        /// <returns>牌的类型(三同,金花....)</returns>
        private ECardsType  GetCardsType() {
            if (IsThree()) {
                return ECardsType.Three;
            }
            bool isThisGold = IsGold();
            bool isThisTractor = IsTractor();
            if (isThisGold&&isThisTractor) {
                return ECardsType.GlodTractor;
            }
            if (isThisGold) {
                return ECardsType.Gold;
            }
            if (isThisTractor) {
                return ECardsType.Tractor;
            }
            if (IsPairs()) {
                return ECardsType.Pairs;
            }
            return ECardsType.Common;
        }
        /// <summary>
        /// 判断牌是否是3张一样
        /// </summary>
        /// <returns></returns>
        private bool IsThree() {
            if (Cards[0].Name==Cards[1].Name &&Cards[0].Name==Cards[2].Name) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断牌是否是花色一样
        /// </summary>
        /// <returns></returns>
        private bool IsGold() {
            if (Cards[0].CardColor==Cards[1].CardColor&&Cards[0].CardColor==Cards[2].CardColor) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否是拖拉机"A23"
        /// </summary>
        /// <returns></returns>
        private bool IsA23() {
            if (CardsSort[2].Name == "A" && CardsSort[1].Name == "3" && CardsSort[0].Name == "2") {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断牌是否是拖拉机
        /// </summary>
        /// <returns></returns>
        private bool IsTractor() {
            if (IsA23()) {
                return true;
            }
                for (int i =2; i>0; i-- ){
                   if( (CardsSort[i].ComparedValue - CardsSort[i - 1].ComparedValue) != 1) {
                        return false;
                    }
                }
                return true;
        }
    /// <summary>
    /// 是否是对子
    /// </summary>
    /// <returns></returns>
        private bool IsPairs() {

            if (CardsSort[2].Name==CardsSort[1].Name&& CardsSort[2].Name!= CardsSort[0].Name ) {
                return true;
            }
            else if (CardsSort[2].Name!= CardsSort[1].Name&& CardsSort[1].Name== CardsSort[0].Name) {
                return true;
            }
            return false;
        }


    }
}
