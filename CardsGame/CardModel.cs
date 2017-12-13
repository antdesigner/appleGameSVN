using System;
using System.Collections.Generic;
using System.Text;

namespace AntDesigner.NetCore.Games {
    public class CardModel
    {
        public string Name { get; set; }
        public string CardColor  { get; set; }
        public CardModel(string name,string cardColor) {
            Name = name;
            CardColor = CardColor;
        }

        public bool IsTheSameWith(CardModel card) {
            if (card.CardColor==CardColor&&card.Name==Name) {
                return true;
            }
            return false;
        }
    }
}
