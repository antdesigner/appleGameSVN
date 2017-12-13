using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    /// <summary>
    /// 玩家出的牌
    /// </summary>
   internal class PutOutCardsCollection
    {
       List<Card> Cards { get; set;}
        /// <summary>
        /// 添加一张牌
        /// </summary>
        /// <param name="card"></param>
       internal void  Add(Card card) {
            Cards.Add(card);
        }
        /// <summary>
        /// 取得最近追加的一张牌
        /// </summary>
        /// <returns></returns>
        internal  Card GetLastCard() {
            return Cards[Cards.Count-1];
        }
    }
}
