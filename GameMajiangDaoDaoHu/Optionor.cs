using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 对需要的牌的操作
    /// </summary>
    public class Optionor {
        public int Number { get; set; }
        /// <summary>
        /// 需要的牌
        /// </summary>
        public CardModel Card { get; set; }
        /// <summary>
        /// 操作(碰,杠,胡等)
        /// </summary>
        public IOption Option { get; set; }
        /// <summary>
        /// 是仅对自己摸上来的牌才有的操作
        /// </summary>
        public bool IsOnlySelf { get; set; }
        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="handCards"></param>
        public bool Do(HandCardManager handCards) {
            return Option.Do(handCards, Card);
        }
        public Optionor(IOption option, CardModel card, bool isOnlySelf = false) {
            Card = card;
            Option = option;
            IsOnlySelf = IsOnlySelf;
        }
        /// <summary>
        /// 是否相同操作
        /// </summary>
        /// <param name="optionor"></param>
        /// <returns></returns>
        public bool IsTheSameWith(Optionor optionor) {
            if (Card.IsTheSameWith(optionor.Card) 
                && Option.Name == optionor.Option.Name
                &&IsOnlySelf==optionor.IsOnlySelf) {
                return true;
            }
            return false;
        }

    }
}
