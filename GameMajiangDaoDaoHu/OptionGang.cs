using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu 
{
    class OptionGang : IOption {
        /// <summary>
        /// 操作名称
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 优先级
        /// </summary>
        public decimal Priority { get; }

        public bool  Do(HandCardManager handCards,CardModel card) {
            
            MaJiangCollection cards = handCards.SleepCards.FindThree(card);
            ///杠自己碰的牌
            if (!(cards is null)){
                if (handCards.IstheSameWhithPikedCard(card)) {
                    cards.Add(handCards.CardPicked);
                    handCards.SetCardPicked(null);

                    return true;
                }
            }
            cards = handCards.StandCards.GetSameCards(card, 4);
            //暗杠
            if (!(cards is null) ){
                handCards.StandCards.RemoveAll(c => c.IsTheSameWith(card));
                handCards.SleepCards.Add(cards);
                return true;
            }
            cards = handCards.StandCards.GetSameCards(card, 3);
            if (!(cards is null)&& handCards.CardPicked.IsTheSameWith(card)) {
                cards.Add(handCards.CardPicked); 
                handCards.SetCardPicked(null);
                handCards.StandCards.RemoveAll(c => c.IsTheSameWith(card));
                handCards.SleepCards.Add(cards);
                return true;
            }

            return false;
        }
        public OptionGang(decimal priority=6,string name = "杠") {
            Name = name;
            Priority = priority;
        }
    }
}
