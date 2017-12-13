using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu {
    internal class StandCardsCollection:MaJiangCollection
    {
        /// <summary>
        /// 加入一张牌
        /// </summary>
        /// <param name="card"></param>
        internal new void  Add(Card card) {
            base.Add(card);
            base.SortByColorThenName();
        }
        /// <summary>
        /// 移除一张牌
        /// </summary>
        /// <param name="cardM"></param>
        /// <returns></returns>
        internal Card Remove(CardModel cardM) {
            Card card = base.FindCard(cardM);
            if (card is null) {
                throw new KeyNotFoundException("没有找到指定牌");
            }
            base.Remove(card);
            return card;
        }
        /// <summary>
        /// 移除多张牌
        /// </summary>
        /// <param name="cardM"></param>
        /// <param name="n"></param>
        internal MaJiangCollection  RemoveCountOfCards(CardModel cardM,int n) {
            MaJiangCollection majiangs = new MaJiangCollection();
            for (int i = base.Count-1; i >=0; i--) {
                if (base[i].IsTheSameWith(cardM)) {
                    majiangs.Add(base[i]);
                    base.Remove(base[i]);
                    n--;
                    if (n==0) {
                        break;
                    }
                }
            }
            return majiangs;
        }
    }
}
