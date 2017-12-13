using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games.GameMajiangDaoDaoHu 
{/// <summary>
/// 碰牌
/// </summary>
    class OptionPeng : IOption {
        public string Name { get; }
        public decimal Priority { get; }
        public bool Do(HandCardManager handCards,Card card){
            MaJiangCollection majians = handCards.StandCards.GetSameCards(card, 2);
            if (!(majians is null)) {
                handCards.StandCards.RemoveCountOf(card, 2);
                majians.Add(card);
                handCards.SleepCards.Add(majians);
            }

            return false;
        }

        public bool Do(HandCardManager handcards, CardModel card) {
            throw new NotImplementedException();
        }

        public  OptionPeng(decimal priority=4, string name = "碰") {
            Name = name;
            Priority = priority;
        }
    }
}
