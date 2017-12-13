using System;
using System.Collections.Generic;

namespace AntDesigner.NetCore.Games {/// <summary>
/// 牌基类类
/// </summary>
    public abstract class ABCard : IComparable {
        /// <summary>
        /// 比较牌大小依据
        /// </summary>
        public int ComparedValue { get; protected set; }
        /// <summary>
        /// 花色(例如:红桃)
        /// </summary>
        public string CardColor { get; protected set; }
        /// <summary>
        /// 点数名称(例如:A)
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// 排序依据
        /// </summary>
        internal int Order { get; set; }
        public virtual int CompareTo(object obj) {
            if (!(obj is ABCard))
                throw new InvalidOperationException("CompareTo: Not a ABCard");
            return ((IComparable)ComparedValue).CompareTo(((ABCard)obj).ComparedValue);
        }

    }
    /// <summary>
    /// 牌类
    /// </summary>
    public class Card : ABCard, IComparable<Card> {
        public Card(int comparedValue_, string CardColor_, string name_) {
            ComparedValue = comparedValue_;
            CardColor = CardColor_;
            Name = name_;

        }
        public override int CompareTo(object obj) {
            if (!(obj is Card))
                throw new InvalidOperationException("CompareTo: Not a Card");
            return ((IComparable)ComparedValue).CompareTo(((Card)obj).ComparedValue);

        }

        public int CompareTo(Card other) {
            return CompareTo((object)other);
        }

        public static bool operator <(Card card1, Card card2) {
            return card1.CompareTo(card2) < 0;
        }
        public static bool operator >(Card card1, Card card2) {
            return card1.CompareTo(card2) > 0;
        }
        public static implicit operator CardModel(Card card) {
            CardModel cardModel = new CardModel(card.Name, card.CardColor);
            return cardModel;
        }
        /// <summary>
        /// /是否相同点数
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool IsTheSameNameOf(Card card) {
            if (this.Name == card.Name) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否相同花色
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool IsTheSameColorOf(Card card) {
            if (this.CardColor == card.CardColor) {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 点数和花色都相同
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool IsTheSameWith(Card card) {
            if (IsTheSameNameOf(card) && IsTheSameColorOf(card)) {
                return true;
            }
            return false;
        }
        public bool IsTheSameWith(CardModel card) {
            if (Name == card.Name && CardColor == card.CardColor) {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 一副牌
    /// </summary>
    public abstract class Cards {/// <summary>
                                 /// 随机数
                                 /// </summary>
        protected Random Random;
        /// <summary>
        /// 牌容器(一副牌)
        /// </summary>
        protected List<Card> CardsList { get; }
        /// <summary>
        /// 罗列牌类名称
        /// </summary>
        protected string[] CardNameList { get; set; }
        /// <summary>
        /// 罗列牌类花色
        /// </summary>
        protected List<string> CardColor { get; set; }
        /// <summary>
        /// 牌类生成规则(循环次数)
        /// </summary>
        protected int Cirlcal { get; set; }
        private object locker;
        /// <summary>
        /// 剩余数量
        /// </summary>
        public int RemaindCount {
            get {

                lock (locker) {
                    return CardsList.Count - Indicator;
                };
            }
        }
        /// <summary>
        /// 一幅牌类
        /// </summary>
        protected Cards() {
            locker = new object();
            ConfigCardsNameAndColor();
            CardsList = new List<Card>();
            Random = new Random();
            BuildNewCards();
            Riffile();
        }
        /// <summary>
        /// 牌类循环生成规则
        /// </summary>
        protected virtual void BuildNewCards() {
            for (int k = 1; k <= Cirlcal; k++) {
                for (int i = 0; i < CardColor.Count; i++) {
                    var color = CardColor[i];
                    for (int j = 0; j < CardNameList.Length; j++) {
                        Card card = new Card(j, color, CardNameList[j]);
                        CardsList.Add(card);
                    }
                }
            }
            Specific();
        }
        /// <summary>
        /// 配置牌类生成规则(用于循环生成)
        /// </summary>
        protected abstract void ConfigCardsNameAndColor();
        /// <summary>
        /// 配置牌类生成规则(非循环生成)
        /// </summary>
        protected abstract void Specific();
        /// <summary>
        /// 添加一张牌(例如:牌中大王)
        /// </summary>
        /// <param name="color_">花色</param>
        /// <param name="name_">点数名称</param>
        protected void AddSinglerCard(string color_, string name_) {
            Card newCard = new Card(CardsList.Count + 1, color_, name_) {
                Order = Random.Next(CardNameList.Length)
            };
            CardsList.Add(newCard);
        }/// <summary>
         /// 添加一张牌
         /// </summary>
         /// <param name="comparedValue">牌的大小比较值</param>
         /// <param name="color_"></param>
         /// <param name="name_"></param>
        protected void AddSinglerCard(int comparedValue, string color_, string name_) {
            Card newCard = new Card(comparedValue, color_, name_) {
                Order = Random.Next(CardNameList.Length)
            };
            CardsList.Add(newCard);
        }
        /// <summary>
        /// 从已经生成的一幅牌里删除牌
        /// </summary>
        /// <param name="color_"></param>
        /// <param name="name_"></param>
        /// <param name="count_"></param>
        protected void DeleteCards(string color_, string name_, int count_ = 1) {
            for (int i = 0; i < CardsList.Count && count_ > 0; i++) {
                if (CardsList[i].CardColor == color_ && CardsList[i].Name == name_) {
                    CardsList.Remove(CardsList[i]);
                    count_--;
                }
            }
        }
        /// <summary>
        /// 洗牌
        /// </summary>
        public void Riffile() {
            for (int i = 0; i < CardsList.Count; i++) {
                CardsList[i].Order = Random.Next(CardsList.Count);
            }
            // CardsList.Sort();
            CardsList.Sort((card1, card2) => {
                return card1.Order.CompareTo(card2.Order);
            });
            Indicator = 0;
        }
        /// <summary>
        /// 发牌指针
        /// </summary>
        private int Indicator { get; set; }
        /// <summary>
        /// 取牌
        /// </summary>
        /// <param name="Count">数量</param>
        /// <returns></returns>
        public List<Card> TackOut(int Count) {
            lock (locker) {
                List<Card> TackOutCards = new List<Card>();
                if (Count <= 0) {
                    return TackOutCards;
                }
                for (int i = 0; i < Count && Indicator < CardsList.Count; i++) {
                    TackOutCards.Add(CardsList[Indicator]);
                    Indicator++;
                }
                return TackOutCards;
            }
        }

    }
    /// <summary>
    /// 标准扑克牌一幅
    /// </summary>
    public class Poker : Cards {
        /// <summary>
        /// 单独添加特殊的扑克牌
        /// </summary>
        protected override void Specific() {
            AddSinglerCard(53, "y", "王"); //小王 调用基类的AddSinglerCard方法单独添加一张牌
            AddSinglerCard(54, "z", "王");//大王
        }
        /// <summary>
        /// 循环定义扑克牌
        /// </summary>
        protected override void ConfigCardsNameAndColor() {
            CardNameList = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            CardColor = new List<string> { "h", "s", "d", "c" };//hearts红桃、spades黑桃、diamonds梅花,clubs方块
            Cirlcal = 1;//循环生成次数点数为1. 共需要生成13*4*1=52张牌
        }
    }
    /// <summary>
    /// 麻将一幅
    /// </summary>
    public class MaJiang : Cards {
        /// <summary>
        /// 循环定义麻将花色和点数
        /// </summary>
        protected override void ConfigCardsNameAndColor() {
            CardNameList = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            CardColor = new List<string> { "w", "t", "l" }; //w"万", t"同", t"条",z中,f发,b,百
            Cirlcal = 4;
        }
        /// <summary>
        /// 单独定义特殊的麻将牌
        /// </summary>
        protected override void Specific() {
            string[] specificCards = new string[] { "z", "f", "b" };//z中,f发,b,百
            for (int i = 0; i < specificCards.Length; i++) {
                for (int j = 0; j < 4; j++) {
                    AddSinglerCard(i + 9, specificCards[i], specificCards[i]);
                }
            }
        }

    }
    public class PokersWithoutKingManger : Poker {
        protected override void Specific() {
            //base.Specific();
        }
    }

    public class MajiangDaoDaoHuManager : Cards {
        protected override void ConfigCardsNameAndColor() {
            CardNameList = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            CardColor = new List<string> { "w", "t", "l" }; //w"万", t"同", t"条",z中,f发,b,百
            Cirlcal = 4;
        }

        protected override void Specific() {
            string[] specificCards = new string[] { "z", "f", "b" };//z中,f发,b,百
            for (int i = 0; i < specificCards.Length; i++) {
                for (int j = 0; j < 4; j++) {
                    AddSinglerCard(i + 9, specificCards[i], specificCards[i]);
                }
            }
        }
    }


}



